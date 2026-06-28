using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CreativeHubWebApp.Models
{

    public class ResourceCollection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Description { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = null!;        // referenca na user

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> ResourceIds { get; set; } = new();  // reference na resource-eve 

        public bool IsPublic { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}