using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace CreativeHubWebApp.Controllers
{

    [ApiController]
    [Route("api/resources/{resourceId}/reviews")]  
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _service;
        public ReviewsController(ReviewService service) => _service = service;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("sub")!;

        [HttpGet]   
        public async Task<IActionResult> GetForResource(string resourceId)
        {
            var reviews = await _service.GetByResourceAsync(resourceId);
            return Ok(reviews.Select(ReviewResponseDto.From));
        }

        [Authorize]
        [HttpPost]   
        public async Task<IActionResult> Add(string resourceId, CreateReviewDto dto)
        {
            try
            {
                var result = await _service.AddReviewAsync(resourceId, UserId, dto);
                return Ok(result);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return BadRequest(new { message = "Već si ocenio ovaj resurs." });
            }
            catch (MongoCommandException)
            {
                return BadRequest(new { message = "Ocena mora biti između 1 i 5." });
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update(string resourceId, CreateReviewDto dto)
        {
            try
            {
                var result = await _service.UpdateReviewAsync(resourceId, UserId, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (MongoCommandException)
            {
                return BadRequest(new { message = "Ocena mora biti između 1 i 5." });
            }
        }

        [Authorize]
        [HttpDelete]   
        public async Task<IActionResult> Delete(string resourceId)
        {
            try
            {
                await _service.DeleteReviewAsync(resourceId, UserId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}