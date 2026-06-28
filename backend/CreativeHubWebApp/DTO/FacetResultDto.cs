namespace CreativeHubWebApp.DTO
{

    public class FacetResultDto
    {
        public List<FacetCount> ByType { get; set; } = new();
        public List<FacetCount> TopTags { get; set; } = new();
    }

    public class FacetCount
    {
        public string Value { get; set; } = null!;   // npr "Brush" ili "fantasy"
        public int Count { get; set; }                // kolko ih ima
    }
}