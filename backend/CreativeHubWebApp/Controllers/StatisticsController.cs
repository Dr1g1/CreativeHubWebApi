using CreativeHubWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreativeHubWebApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly StatisticsService _service;
        public StatsController(StatisticsService service) => _service = service;

        [HttpGet("resources/{id}")]  
        public async Task<IActionResult> ResourceDetail(string id)
        {
            var detail = await _service.GetResourceDetailAsync(id);
            return detail is null ? NotFound() : Ok(detail);
        }

        [HttpGet("top-creators")]  
        public async Task<IActionResult> TopCreators([FromQuery] int limit = 10)
            => Ok(await _service.GetTopCreatorsAsync(limit));

        [HttpGet("rating-distribution")]   
        public async Task<IActionResult> RatingDistribution()
            => Ok(await _service.GetRatingDistributionAsync());
    }
}