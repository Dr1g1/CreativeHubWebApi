using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CreativeHubWebApp.Models
{
    public class Resource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Description { get; set; } = "";

        [BsonRepresentation(BsonType.String)]
        public ResourceType Type { get; set; }

        public List<string> Tags { get; set; } = new();

        [BsonRepresentation(BsonType.String)]
        public ContentType ContentType { get; set; } = ContentType.File;

        [BsonRepresentation(BsonType.ObjectId)]
        public string? FileId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? ThumbnailFileId { get; set; }

        public string FileFormat { get; set; } = "";
        public long FileSizeBytes { get; set; }

        public List<string> Colors { get; set; } = new();

        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = null!;

        public int Downloads { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> PreviewImageIds { get; set; } = new();
    }
}