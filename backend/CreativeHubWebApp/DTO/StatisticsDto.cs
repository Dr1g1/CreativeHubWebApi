namespace CreativeHubWebApp.DTO
{
    public class TopResourceDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Downloads { get; set; } 
        public double AverageRating { get; set; }
    }

    public class TopCreatorDto
    {
        public string CreatorId { get; set; } = null!;
        public string Username { get; set;} = "";
        public int ResourceCount { get; set; }
        public int TotalDownloads { get; set; }
    }
    public class RatingBucketDto
    {
        public string Range { get; set; } = null!;   
        public int Count { get; set; }
    }
}
