using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthometer_API.Models;

public class MedicalVisit
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")] public string? Name { get; set; } = null;
    [BsonElement("category")] public string? Category { get; set; } = null;
    [BsonElement("date")] public DateTime? Date { get; set; }
    [BsonElement("notification")] public string? Notification { get; set; }
}