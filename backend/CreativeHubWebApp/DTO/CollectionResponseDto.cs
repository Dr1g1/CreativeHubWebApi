using CreativeHubWebApp.Models;

namespace CreativeHubWebApp.DTO
{

    public class CollectionResponseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = "";
        public string OwnerId { get; set; } = null!;
        public List<string> ResourceIds { get; set; } = new();
        public bool IsPublic { get; set; }
        public int ResourceCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public static CollectionResponseDto From(ResourceCollection c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            OwnerId = c.OwnerId,
            ResourceIds = c.ResourceIds,
            IsPublic = c.IsPublic,
            ResourceCount = c.ResourceIds.Count,
            CreatedAt = c.CreatedAt
        };
    }
}