using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Healthometer_API.Services;
public class DocumentsService
{
    private readonly IMongoCollection<User> _documentsCollection;

    public DocumentsService (
        IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);
        
        _documentsCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);

    }

    public async Task<List<Document>>? GetAsync(string id)
    {
        var user = await _documentsCollection
            .Find(e => e.Id == id)
            .FirstOrDefaultAsync();
        var docs = user.Docs;
        return docs;
    }

    public async Task<User>? PutAsync(string id, Document newDocument) =>
        await _documentsCollection.FindOneAndUpdateAsync(
            user => user.Id == id, Builders<User>.Update.Push("docs", newDocument)
            );
}