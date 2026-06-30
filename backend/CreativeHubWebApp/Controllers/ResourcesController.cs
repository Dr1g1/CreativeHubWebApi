using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreativeHubWebApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly ResourceService _service;
        public ResourcesController(ResourceService service) => _service = service;

        // pomocna fja da izvuce userid iz tokena
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("sub")!;

        [HttpGet]   
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]  
        public async Task<IActionResult> GetById(string id)
        {
            var r = await _service.GetByIdAsync(id);
            return r is null ? NotFound() : Ok(r);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            [FromForm] CreateResourceDto dto, IFormFile file, [FromForm] List<IFormFile>? previews)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { message = "Fajl je obavezan." });

            using var mainStream = file.OpenReadStream();

           
            var previewData = new List<(Stream stream, string fileName)>();
            var openStreams = new List<Stream>();
            if (previews is not null)
            {
                foreach (var p in previews)
                {
                    if (p.Length > 0)
                    {
                        var s = p.OpenReadStream();
                        openStreams.Add(s);
                        previewData.Add((s, p.FileName));
                    }
                }
            }

            try
            {
                var result = await _service.CreateWithFileAsync(
                    dto, UserId, mainStream, file.FileName, file.Length, previewData);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            finally
            {
                foreach (var s in openStreams) s.Dispose();
            }
        }

        [Authorize]
        [HttpPost("palette")]
        public async Task<IActionResult> CreatePalette(
            [FromForm] CreateResourceDto dto, [FromForm] List<IFormFile>? previews)
        {
            var previewData = new List<(Stream stream, string fileName)>();
            var openStreams = new List<Stream>();
            if (previews is not null)
            {
                foreach (var p in previews)
                {
                    if (p.Length > 0)
                    {
                        var s = p.OpenReadStream();
                        openStreams.Add(s);
                        previewData.Add((s, p.FileName));
                    }
                }
            }

            try
            {
                var result = await _service.CreatePaletteAsync(dto, UserId, previewData);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            finally
            {
                foreach (var s in openStreams) s.Dispose();
            }
        }

        [HttpGet("{id}/download")]   
        public async Task<IActionResult> Download(string id)
        {
            var result = await _service.DownloadAsync(id);
            if (result is null) return NotFound();
            return File(result.Value.data, "application/octet-stream", result.Value.fileName);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id, UserId);
                return ok ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        [HttpGet("preview/{fileId}")]   
        public async Task<IActionResult> GetPreview(string fileId)
        {
            try
            {
                var (data, fileName) = await _service.GetPreviewAsync(fileId);
                return File(data, GuessContentType(fileName));   
            }
            catch
            {
                return NotFound();
            }
        }

        // pogadja tip slike iz ekstenzije da je browser prikaze umesto da je skida
        private static string GuessContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}