using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
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
            using var image = await Image.LoadAsync(source);
            image.Mutate(operation => operation.AutoOrient());

            if (image.Width > MaximumImageDimension || image.Height > MaximumImageDimension)
            {
                image.Mutate(operation => operation.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(MaximumImageDimension, MaximumImageDimension)
                }));
            }

            while (true)
            {
                for (var quality = 85; quality >= 20; quality -= 5)
                {
                    var output = new MemoryStream();
                    await image.SaveAsWebpAsync(output, new WebpEncoder
                    {
                        Quality = quality,
                        FileFormat = WebpFileFormatType.Lossy
                    });

                    if (output.Length <= MaximumImageBytes)
                    {
                        output.Position = 0;
                        return new PreparedUpload(output, "image/webp", ".webp");
                    }

                    await output.DisposeAsync();
                }

                if (image.Width <= MinimumImageDimension && image.Height <= MinimumImageDimension)
                    throw new InvalidOperationException("The image could not be compressed below 100 KB.");

                var nextWidth = Math.Max(MinimumImageDimension, (int)(image.Width * 0.8));
                var nextHeight = Math.Max(MinimumImageDimension, (int)(image.Height * 0.8));
                image.Mutate(operation => operation.Resize(nextWidth, nextHeight));
            }
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
