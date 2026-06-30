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
        public string ResourceId { get; set; } = null!;   

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;        

        public int Rating { get; set; }                 
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}