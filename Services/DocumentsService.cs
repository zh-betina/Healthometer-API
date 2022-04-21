using System.Collections.Immutable;
using Healthometer_API.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
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

    public async Task<User>? DeleteAsync(string userId, string docId)
    {
        
        //TODO refactor !
        var userFilter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId));
        userFilter &= Builders<User>.Filter.ElemMatch(user => user.Docs, Builders<Document>.Filter.Eq(document =>
            document.Id, docId));

        var filter = new BsonDocument("_id", ObjectId.Parse(userId));
        var update = Builders<User>.Update.PullFilter("docs",
            Builders<Document>.Filter.Eq(doc => doc.Id, docId));

        var docEntry = await _documentsCollection
            .Find(userFilter).FirstOrDefaultAsync();
        var test = docEntry.Docs.ToList();

        var docIndex = test.FindIndex(doc => doc.Id == docId);
        
        if (docIndex > -1)
        {
            var pathFileToDelete = test[docIndex].Path;
            if (pathFileToDelete is not null) File.Delete(pathFileToDelete);
        }

        var result = await _documentsCollection.FindOneAndUpdateAsync(filter, update);
        
        return result;
    }

    public async Task<UpdateResult>? ModifyAsync(string userId, string docId, Document modifiedDocument)
    {
        var filter = Builders<User>.Filter.Eq(user => user.Id, userId)
                     & Builders<User>.Filter.ElemMatch(user => user.Docs, Builders<Document>.Filter.Eq(document =>
                         document.Id, docId));
        var update = Builders<User>.Update.Set(user => user.Docs[-1], modifiedDocument);
        var result = await _documentsCollection.UpdateOneAsync(filter, update);
        return result;
    }

    
}