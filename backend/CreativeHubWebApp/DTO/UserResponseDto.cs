using CreativeHubWebApp.Models;

namespace CreativeHubWebApp.DTO
{

    public class UserResponseDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DisplayName { get; set; } = "";
        public string Bio { get; set; } = "";

        public static UserResponseDto From(User u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            DisplayName = u.DisplayName,
            Bio = u.Bio
        };
    }
}