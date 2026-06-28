using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreativeHubWebApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CollectionsController : ControllerBase
    {
        private readonly CollectionService _service;
        public CollectionsController(CollectionService service) => _service = service;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("sub")!;

        [HttpGet("{id}")]  
        public async Task<IActionResult> GetById(string id)
        {
            var c = await _service.GetByIdAsync(id);
            return c is null ? NotFound() : Ok(c);
        }

        [Authorize]
        [HttpGet("mine")]  
        public async Task<IActionResult> GetMine() => Ok(await _service.GetByOwnerAsync(UserId));

        [Authorize]
        [HttpPost]   
        public async Task<IActionResult> Create(CreateCollectionDto dto)
        {
            var result = await _service.CreateAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize]
        [HttpPost("{id}/resources/{resourceId}")] 
        public async Task<IActionResult> AddResource(string id, string resourceId)
        {
            try
            {
                await _service.AddResourceAsync(id, resourceId, UserId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [Authorize]
        [HttpDelete("{id}/resources/{resourceId}")] 
        public async Task<IActionResult> RemoveResource(string id, string resourceId)
        {
            try
            {
                await _service.RemoveResourceAsync(id, resourceId, UserId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
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
            catch (UnauthorizedAccessException ex) { return Forbid(); }
        }
    }
}