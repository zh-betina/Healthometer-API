using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthometer_API.Models;

public class DocFile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; } = null;
    public Document Doc { get; set; } = null;
    public IFormFile? FileContent { get; set; }
}