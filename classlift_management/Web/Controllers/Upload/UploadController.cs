using Amazon.S3;
using Amazon.S3.Model;
using Core.R2;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Upload
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        private readonly R2StorageService _storage;

        public UploadController(R2StorageService storage)
        {
            _storage = storage;
        }


        [HttpPost("single")]
        public async Task<IActionResult> UploadSingle(IFormFile file, string folder, string fileName)
        {
            if (file == null)
                return BadRequest("File is required.");

            var url = await _storage.UploadAsync(file, folder, fileName);

            return Ok(new { url });
        }


        //[HttpPost("image")]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File is missing.");

        //    var bucket = _config["CloudflareR2:BucketName"];
        //    var publicUrl = _config["CloudflareR2:PublicUrl"];

        //    // Generate unique file name
        //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        //    // Upload request
        //    var request = new PutObjectRequest
        //    {
        //        BucketName = bucket,
        //        Key = fileName,
        //        InputStream = file.OpenReadStream(),
        //        ContentType = file.ContentType,
        //    };

        //    await _s3.PutObjectAsync(request);

        //    // Final public URL
        //    var url = $"{publicUrl}/{fileName}";

        //    return Ok(new { url });
        //}
    }

}
