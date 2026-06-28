using CreativeHubWebApp.Models;

namespace CreativeHubWebApp.DTO
{

    public class ReviewResponseDto
    {
        public string Id { get; set; } = null!;
        public string ResourceId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public static ReviewResponseDto From(Review r) => new()
        {
            Id = r.Id,
            ResourceId = r.ResourceId,
            UserId = r.UserId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}