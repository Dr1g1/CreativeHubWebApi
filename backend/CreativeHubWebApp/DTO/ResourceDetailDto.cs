namespace CreativeHubWebApp.DTO
{

    public class ResourceDetailDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public int Downloads { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public CreatorDto? Creator { get; set; }
        public List<ReviewWithUserDto> Reviews { get; set; } = new();
    }

    public class CreatorDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = "";
    }

    public class ReviewWithUserDto
    {
        public string Id { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
        public string Username { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}