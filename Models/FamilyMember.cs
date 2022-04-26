using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthometer_API.Models;

public class FamilyMember
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")] public string? Name { get; set; }
    [BsonElement("icon")] public string? Icon { get; set; }
    [BsonElement("family_link")] public string? FamilyLink { get; set; }
    [BsonElement("docs")] public List<Document> Docs { get; set; } = new List<Document>();
    [BsonElement("categories")] public List<string> Categories { get; set; } = new List<string>();
    [BsonElement("medical_visits")] public List<MedicalVisit> MedicalVisits { get; set; } = new List<MedicalVisit>();
}