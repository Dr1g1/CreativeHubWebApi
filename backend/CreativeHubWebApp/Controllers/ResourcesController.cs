using CreativeHubWebApp.DTO;
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

        [Authorize]   // mora korisnik da bude ulogovan
        [HttpPost("upload")]   
        public async Task<IActionResult> Upload(
            [FromForm] CreateResourceDto dto, IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { message = "Fajl je obavezan." });

            using var stream = file.OpenReadStream();
            var result = await _service.CreateWithFileAsync(
                dto, UserId, stream, file.FileName, file.Length);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize]
        [HttpPost("palette")]   // za paletu posot njoj ne treba fajl
        public async Task<IActionResult> CreatePalette(CreateResourceDto dto)
        {
            var result = await _service.CreatePaletteAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
    }
}