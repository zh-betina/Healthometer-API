using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthometer_API.Models;

[BsonIgnoreExtraElements]
public class Document
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();
    [BsonElement("name")] public string? Name { get; set; }
    [BsonElement("date")] public DateTime? Date { get; set; }
    [BsonElement("status")] public Status? Status { get; set; }
    [BsonElement("format")] public string? Format { get; set; }
    [BsonElement("category")] public string? Category { get; set; }
    [BsonElement("path")] public string? Path { get; set; }
}