using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthometer_API.Models;

[BsonIgnoreExtraElements] 
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;

    [BsonElement("email")] public string? Email { get; set; }
    [BsonElement("name")] public string? Name { get; set; }
    [BsonElement("password")] public string? Password { get; set; }
    [BsonElement("phone_no")] public string? PhoneNo { get; set; }
    [BsonElement("lang")] public string? Lang { get; set; } 
    [BsonElement("icon")] public string? Icon { get; set; }
    [BsonElement("taken_space")] public float? TakenSpace { get; set; }
    [BsonElement("docs")] public List<Document> Docs { get; set; } = null;
}