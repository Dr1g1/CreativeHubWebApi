using CreativeHubWebApp.Models;

namespace CreativeHubWebApp.DTO
{

    public class ResourceSearchQueryDto
    {
        public string? Text { get; set; }// pretraga po naslovu ili opisu
        public ResourceType? Type { get; set; }// filter po tipu opciona
        public List<string>? Tags { get; set; }// filter po tagovima opciona
        public string SortBy { get; set; } = "newest";// newest ili downloafs ili rating
        public int Page { get; set; } = 1;// za paginaviju
        public int PageSize { get; set; } = 20;
        public string? ExcludeOwnerId { get; set; }   
    }
}