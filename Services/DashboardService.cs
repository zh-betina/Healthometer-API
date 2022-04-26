using System.Threading.Tasks;
using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Healthometer_API.Services;

public class DashboardService
{
    private readonly IMongoCollection<User> _usersCollection;

    public DashboardService (
        IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);
        
        _usersCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<Dashboard> GetAsync(string userId)
    {
        var user = await _usersCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
        //TODO Need to sort out the visits that are regular
        var dashboard = new Dashboard
        {
            RegularVisits = user.MedicalVisits,
            TakenSpace = user.TakenSpace,
            Documents = user.Docs,
            Family = user.FamilyMembers
        };

        return dashboard;
    }
}