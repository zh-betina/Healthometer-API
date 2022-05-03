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
    [BsonElement("lang")] public Language? Language = Models.Language.EN;
    [BsonElement("icon")] public string? Icon { get; set; } = "";
    [BsonElement("taken_space")] public long? TakenSpace { get; set; } = 0;
    [BsonElement("docs")] public List<Document> Docs { get; set; } = new List<Document>();
    [BsonElement("categories")] public List<string> Categories { get; set; } = new List<string>();
    [BsonElement("medical_visits")] public List<MedicalVisit> MedicalVisits { get; set; } = new List<MedicalVisit>();
    [BsonElement("family")] public List<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
}