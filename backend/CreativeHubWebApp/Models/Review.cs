using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CreativeHubWebApp.Models
{

    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ResourceId { get; set; } = null!;   // referenca na resource

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;        // referenca na usera

        public int Rating { get; set; }                 
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}