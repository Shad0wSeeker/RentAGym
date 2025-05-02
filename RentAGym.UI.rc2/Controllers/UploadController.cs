using Microsoft.AspNetCore.Mvc;
using RentAGym.Application.Interfaces;
using System.Net;

namespace RentAGym.UI.rc2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            if (Request.Form.Files.Count == 0)
                return BadRequest("Файл не предоставлен");

            var file = Request.Form.Files[0]; // Берём первый файл

            try
            {
                using var stream = file.OpenReadStream();
                var filePath = await _fileService.SaveFileAsync(stream, file.FileName);
                return Ok(new { filePath });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Ошибка при загрузке файла: {ex.Message}");
            }
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> UploadFiles()
        {
            if (Request.Form.Files.Count == 0)
                return BadRequest("Файлы не предоставлены");

            var results = new List<object>();
            foreach (var file in Request.Form.Files)
            {
                try
                {
                    using var stream = file.OpenReadStream();
                    var filePath = await _fileService.SaveFileAsync(stream, file.FileName);
                    results.Add(new { file.FileName, filePath });
                }
                catch (Exception ex)
                {
                    results.Add(new { file.FileName, Error = ex.Message });
                }
            }

            return Ok(results);
        }
    
    }
}
