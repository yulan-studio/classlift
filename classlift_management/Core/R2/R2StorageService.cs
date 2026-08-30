using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Core.R2
{
    public class R2StorageService
    {
        private const int MaximumImageBytes = 100 * 1024;
        private const int MaximumImageDimension = 1600;
        private const int MinimumImageDimension = 128;
        private readonly CloudflareR2Options _options;
        private readonly IAmazonS3 _s3Client;

        public R2StorageService(IOptions<CloudflareR2Options> options)
        {
            _options = options.Value;

            var config = new AmazonS3Config
            {
                ServiceURL = $"https://{_options.AccountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true // REQUIRED for R2
            };

            _s3Client = new AmazonS3Client(
                _options.AccessKey,
                _options.SecretKey,
                config
            );
        }

        public async Task<string> UploadAsync(IFormFile file, string folder, string file_name)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Empty file");

            await using var upload = await PrepareUploadAsync(file, file.ContentType);
            string fileName = $"{file_name}{upload.FileExtension}";

            // Build the object key (folder + filename)
            string key = $"{folder}/{fileName}".Replace("//", "/");

            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = upload.Stream,
                ContentType = upload.ContentType,
                UseChunkEncoding = false
            };

            await _s3Client.PutObjectAsync(request);

            // Return public URL or save Key to DB
            return $"{_options.PublicUrl}/{folder}/{fileName}";
        }

        public async Task<string> UploadToKeyAsync(
            IFormFile file,
            string objectKey,
            string contentType)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The uploaded file is empty.", nameof(file));

            var key = objectKey.Trim('/').Replace("//", "/");

            await using var upload = await PrepareUploadAsync(file, contentType);
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = upload.Stream,
                ContentType = upload.ContentType,
                UseChunkEncoding = false
            };

            await _s3Client.PutObjectAsync(request);

            return GetPublicUrl(key);
        }

        private static async Task<PreparedUpload> PrepareUploadAsync(
            IFormFile file,
            string? contentType)
        {
            var normalizedContentType = contentType?.Split(';')[0].Trim().ToLowerInvariant();
            var isSupportedImage = normalizedContentType is "image/jpeg" or "image/png" or "image/webp";

            if (!isSupportedImage || file.Length <= MaximumImageBytes)
            {
                return new PreparedUpload(
                    file.OpenReadStream(),
                    contentType ?? "application/octet-stream",
                    Path.GetExtension(file.FileName));
            }

            await using var source = file.OpenReadStream();
            using var original = SKBitmap.Decode(source)
                ?? throw new InvalidOperationException("The uploaded image is invalid.");
            var current = ResizeToFit(original, MaximumImageDimension);

            try
            {
                while (true)
                {
                    for (var quality = 85; quality >= 20; quality -= 5)
                    {
                        using var encoded = current.Encode(SKEncodedImageFormat.Webp, quality)
                            ?? throw new InvalidOperationException("The image could not be encoded.");

                        if (encoded.Size <= MaximumImageBytes)
                        {
                            var output = new MemoryStream(encoded.ToArray(), writable: false);
                            return new PreparedUpload(output, "image/webp", ".webp");
                        }
                    }

                    if (current.Width <= MinimumImageDimension && current.Height <= MinimumImageDimension)
                        throw new InvalidOperationException("The image could not be compressed below 100 KB.");

                    var nextWidth = Math.Max(MinimumImageDimension, (int)(current.Width * 0.8));
                    var nextHeight = Math.Max(MinimumImageDimension, (int)(current.Height * 0.8));
                    var resized = current.Resize(
                        new SKImageInfo(nextWidth, nextHeight),
                        new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None))
                        ?? throw new InvalidOperationException("The image could not be resized.");

                    current.Dispose();
                    current = resized;
                }
            }
            finally
            {
                current.Dispose();
            }
        }

        private static SKBitmap ResizeToFit(SKBitmap source, int maximumDimension)
        {
            if (source.Width <= maximumDimension && source.Height <= maximumDimension)
                return source.Copy();

            var scale = Math.Min(
                (double)maximumDimension / source.Width,
                (double)maximumDimension / source.Height);
            var width = Math.Max(1, (int)Math.Round(source.Width * scale));
            var height = Math.Max(1, (int)Math.Round(source.Height * scale));

            return source.Resize(
                new SKImageInfo(width, height),
                new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None))
                ?? throw new InvalidOperationException("The image could not be resized.");
        }

        private sealed record PreparedUpload(
            Stream Stream,
            string ContentType,
            string FileExtension) : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => Stream.DisposeAsync();
        }

        public string GetPublicUrl(string objectKey) =>
            $"{_options.PublicUrl.TrimEnd('/')}/{objectKey.TrimStart('/')}";

        public async Task UploadTextAsync(
            string objectKey,
            string content)
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey.Trim('/').Replace("//", "/"),
                ContentBody = content,
                ContentType = "text/plain; charset=utf-8",
                UseChunkEncoding = false
            };

            await _s3Client.PutObjectAsync(request);
        }

        public async Task<string?> GetTextAsync(string objectKey)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = objectKey.Trim('/').Replace("//", "/")
                };

                using var response = await _s3Client.GetObjectAsync(request);
                using var reader = new StreamReader(response.ResponseStream);
                return await reader.ReadToEndAsync();
            }
            catch (AmazonS3Exception exception)
                when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
