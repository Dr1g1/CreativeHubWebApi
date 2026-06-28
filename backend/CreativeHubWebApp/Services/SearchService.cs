using CreativeHubWebApp.DTO;
using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Repositories;

namespace CreativeHubWebApp.Services
{

    public class SearchService
    {
        private readonly SearchRepository _search;
        public SearchService(SearchRepository search) => _search = search;

        public async Task<PagedResultDto<ResourceResponseDto>> SearchAsync(ResourceSearchQueryDto q)
        {
            var result = await _search.SearchAsync(q);
            return new PagedResultDto<ResourceResponseDto>
            {
                Items = result.Items.Select(ResourceResponseDto.From).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public Task<FacetResultDto> GetFacetsAsync() => _search.GetFacetsAsync();
    }
}