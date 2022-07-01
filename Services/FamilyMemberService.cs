using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
        var familyMembers = await _familyMembersCollection.Find(
                Builders<User>.Filter.Eq(user => user.Id, userId))
            .FirstOrDefaultAsync();
            //REFACTOR LATER - FIND OUT HOW TO CAST FROM BSON DOCUMENT...
            // .Project(
            //     Builders<User>.Projection
            //         .Include(user => user.FamilyMembers)
            //         .Exclude(user => user.Id))
            // .ToListAsync();
        
        return familyMembers.FamilyMembers;
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