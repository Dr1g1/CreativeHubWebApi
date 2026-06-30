namespace CreativeHubWebApp.DTO
{
    public class UpdateResourceDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public List<string> Colors { get; set; } = new();
    }
}
