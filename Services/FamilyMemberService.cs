using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Healthometer_API.Services;

public class FamilyMemberService
{
    private readonly IMongoCollection<User> _familyMembersCollection;

    public FamilyMemberService(
        IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _familyMembersCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<FamilyMember>> GetAsync(string userId)
    {
        var projection = Builders<User>.Projection
            .Exclude("_id")
            .Include("family");

        var familyMembers = await _familyMembersCollection
            .Find(user => user.Id == userId)
            .Project(projection)
            .FirstOrDefaultAsync();

        List<FamilyMember> familyMembersList = BsonSerializer.Deserialize<List<FamilyMember>>(familyMembers[0].ToJson());

        return familyMembersList;
    }
    
    public async Task<List<Document>> GetDocsAsync(string userId, string familyId)
    {
        var projection = Builders<User>.Projection.Expression(
            user => user.FamilyMembers.Where(family => family.Id == familyId));

        var familyMember = await _familyMembersCollection
            .Find(user => user.Id == userId)
            .Project(projection)
            .FirstOrDefaultAsync();
        
        List<FamilyMember> familyMemberList = familyMember.ToList();
        List<Document> familyDocs = familyMemberList[0].Docs;

        if (familyMember is not null)
        {
            FamilyMember familyJson = BsonSerializer.Deserialize<FamilyMember>(familyMemberList[0].ToJson());
            return familyJson.Docs;
        }
        
        return familyDocs;
    }

    public async Task<string> PostAsync(string userId, FamilyMember newFamilyMember)
    {
        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId));
        var update = Builders<User>.Update.Push("family", newFamilyMember);
        await _familyMembersCollection.UpdateOneAsync(filter, update);
        return "ok";
    }

    public async Task<string> DeleteAsync(string userId, string familyMemberId)
    {
        var userFilter = Builders<User>.Filter.Eq(user => user.Id, userId);
        var removalFilter = Builders<FamilyMember>.Filter.Where(member => member.Id == familyMemberId);
        var pullFilter = Builders<User>.Update.PullFilter(user => user.FamilyMembers, removalFilter);
        await _familyMembersCollection.UpdateOneAsync(userFilter, pullFilter);
        return "ok";
    }
}