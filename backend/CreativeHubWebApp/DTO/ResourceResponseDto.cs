using CreativeHubWebApp.Models;

namespace CreativeHubWebApp.DTO
{

    public class ResourceResponseDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public string FileFormat { get; set; } = "";
        public long FileSizeBytes { get; set; }
        public string OwnerId { get; set; } = null!;
        public int Downloads { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> PreviewImageIds { get; set; } = new();

        // pomocna fja koja pretvara resource model u ovaj dto iznad
        public static ResourceResponseDto From(Resource r) => new()
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            Type = r.Type.ToString(),
            Tags = r.Tags,
            Colors = r.Colors,
            FileFormat = r.FileFormat,
            FileSizeBytes = r.FileSizeBytes,
            OwnerId = r.OwnerId,
            Downloads = r.Downloads,
            AverageRating = r.AverageRating,
            ReviewCount = r.ReviewCount,
            CreatedAt = r.CreatedAt,
            PreviewImageIds = r.PreviewImageIds,
        };
    }
}