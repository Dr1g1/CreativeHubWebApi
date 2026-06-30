using CreativeHubWebApp.Models;

namespace CreativeHubWebApp.DTO
{

    public class CreateResourceDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = "";
        public ResourceType Type { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<string> Colors { get; set; } = new();   
    }
}