using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreativeHubWebApp.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _service;
        public SearchController(SearchService service) => _service = service;

        // POST jer pretraga ima vise kriterijuma 
        [HttpPost]   
        public async Task<IActionResult> Search(ResourceSearchQueryDto query)
            => Ok(await _service.SearchAsync(query));

        [HttpGet("facets")]   
        public async Task<IActionResult> Facets()
            => Ok(await _service.GetFacetsAsync());
    }
}