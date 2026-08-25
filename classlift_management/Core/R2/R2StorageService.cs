using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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

            string fileName = $"{file_name}{Path.GetExtension(file.FileName)}";

            // Build the object key (folder + filename)
            string key = $"{folder}/{fileName}".Replace("//", "/");


            using (var stream = file.OpenReadStream())
            {
                var request = new PutObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = key,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    //AutoCloseStream = true,
                    UseChunkEncoding = false
                    
                };

                await _s3Client.PutObjectAsync(request);
            }

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

            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                UseChunkEncoding = false
            };

            await _s3Client.PutObjectAsync(request);

            return GetPublicUrl(key);
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
