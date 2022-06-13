using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthometer_API.Models;

public class DocFile
{
    public IFormFile? FileContent { get; set; }
}