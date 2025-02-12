using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly static string _privateDirecotryName = "PrivateFiles";

        [HttpGet]
        [ResponseCache(VaryByQueryKeys = new[] { "fileName"}, Duration =1200)]
        public IActionResult GetFile([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new BadRequestException("File name must be declared");
            }

            var root = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(root, _privateDirecotryName, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileContent = System.IO.File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string? contentType);

            return File(fileContent, contentType is not null ? contentType : "application/octet-stream", fileName);
        }


        [HttpPost]
        public ActionResult Upload([FromForm] IFormFile file)
        {
            if (file is null || file.Length is 0)
            {
                return BadRequest("File doesn't exists");
            }

            var root = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(root, _privateDirecotryName, file!.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
            stream.Close();

            return Ok();
        }
    }
}
