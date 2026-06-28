namespace CreativeHubWebApp.DTO
{
    public class AuthResponseDto
    {
        // ono sto vracamo posle uspesne registracije ili logina
        public string Token { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
