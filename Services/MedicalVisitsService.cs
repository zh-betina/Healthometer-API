using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Healthometer_API.Services;

public class MedicalVisitsService
{
    private readonly IMongoCollection<User> _visitsCollection;

    public MedicalVisitsService(IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _visitsCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<User> PostAsync(string userId, MedicalVisit newVisit, string? familyMemberId)
    {
        //if (familyMemberId != null)
        //{
        //    Console.WriteLine("TO DO");
        //    return 
        //}
            var result = await _visitsCollection.FindOneAndUpdateAsync(
                user => user.Id == userId, Builders<User>.Update.Push("medical_visits", newVisit));
            return result;
    }

}