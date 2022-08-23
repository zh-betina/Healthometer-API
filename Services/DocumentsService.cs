using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Healthometer_API.Services;

public class DocumentsService
{
    private readonly IMongoCollection<User> _documentsCollection;

    public DocumentsService(
        IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _documentsCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);

    }
    
    public Document PrepareDocInfoForDatabase(Document docInfo)
    {
        var docToSendToDb = new Document
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Format = docInfo.Format,
            Path = docInfo.Path,
            Date = docInfo.Date,
            Category = docInfo.Category,
            Name = docInfo.Name,
            Status = docInfo.Status
        };

        return docToSendToDb;
    }

    public async Task<List<Document>>? GetAsync(string id)
    {
        var user = await _documentsCollection
            .Find(e => e.Id == id)
            .FirstOrDefaultAsync();
        var docs = user.Docs;
        return docs;
    }

    public async Task<User>? PostAsync(Document newDocument, string userId)
    {
        var docInfoForDatabase = PrepareDocInfoForDatabase(newDocument);
        var result = await _documentsCollection.FindOneAndUpdateAsync(
            user => user.Id == userId, Builders<User>.Update.Push("docs", docInfoForDatabase)
        );
        
        return result;
    }

    public async Task<List<string>>? DeleteAsync(string userId, List<string> docsToRemove)
    {
        //TODO refactor !
        List<string> result = new List<string>();
    foreach(var docId in docsToRemove)
    {
        var docFilter = Builders<Document>.Filter.Eq(doc => doc.Id, docId);

        var doc = await _documentsCollection.Find(
                Builders<User>.Filter.ElemMatch(el => el.Docs, docFilter))
            .Project(
                Builders<User>.Projection.Include(e => e.Docs)
                    .Exclude(e => e.Id)
                    .Exclude(e => e.Docs)
                    .ElemMatch(user => user.Docs, docFilter)
            ).SingleOrDefaultAsync();

        BsonValue docs = doc["docs"];
        List<Document> docObj = BsonSerializer.Deserialize<List<Document>>(docs.ToJson());
        var filePath = docObj[0].Path;
        
        if (filePath is not null) File.Delete(filePath);
        
        var update =
            Builders<User>.Update.PullFilter(user => user.Docs, Builders<Document>.Filter.Eq(doc => doc.Id, docId));
        var deleteResult = await _documentsCollection.UpdateOneAsync(user => user.Id.Equals(userId), update);


            //CORRECT VERIFICATION!
            if (deleteResult is not null)
            {
                result.Add("Ok");
            }
            else
            {
                result.Add("Not ok");
            }
        }

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