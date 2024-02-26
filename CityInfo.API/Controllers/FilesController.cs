 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider provider)
        {
            _fileExtensionContentTypeProvider = provider ?? 
                throw new ArgumentNullException(nameof(provider));
        }

        [HttpGet("{fileId}")]
        public IActionResult GetFile(string fileId)
        {
            if(!System.IO.File.Exists(fileId))
            {
                return NotFound();
            }

            if(!_fileExtensionContentTypeProvider.TryGetContentType(fileId, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(fileId);
            return File(bytes, contentType, Path.GetFileName(fileId));
        }
    }
}
