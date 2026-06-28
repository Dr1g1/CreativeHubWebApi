namespace CreativeHubWebApp.DTO
{
    //mora da koristimo genericku klasu jer moze da bude i resurs i korisnik itd.
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public long TotalCount { get; set; }   // ukupno rezultata kolko ima
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
