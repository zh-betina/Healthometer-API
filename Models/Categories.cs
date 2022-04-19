using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthometer_API.Models;

public class Categories
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null;
    
    [BsonRepresentation(BsonType.Array)]
    public List<string>? CategoriesList { get; set; }
}