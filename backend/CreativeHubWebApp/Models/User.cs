using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CreativeHubWebApp.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string DisplayName { get; set; } = "";
        public string Bio { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string? AvatarFileId { get; set; }

        public string Role { get; set; } = "user";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
