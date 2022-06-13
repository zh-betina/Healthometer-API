using System.Threading.Tasks;
using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Healthometer_API.Services;

public class DashboardService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly MedicalVisitsService _visitsService;

    public DashboardService (
        IOptions<UsersDatabaseSettings> usersDatabaseSettings, MedicalVisitsService visitsService)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);
        
        _usersCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);

        _visitsService = visitsService;
    }

    public async Task<Dashboard> GetAsync(string userId)
    {
        var user = await _usersCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
        List<MedicalVisit> regularVisits = _visitsService.sortOutRegularVisits(user.MedicalVisits);
        Dashboard dashboard = new Dashboard
        {
            RegularVisits = regularVisits,
            TakenSpace = user.TakenSpace,
            Documents = user.Docs,
            Family = user.FamilyMembers,
            UserName = user.Name
        };

        return dashboard;
    }
}