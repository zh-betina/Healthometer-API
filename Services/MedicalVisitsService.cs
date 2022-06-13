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

    public async Task<List<MedicalVisit>> GetAsync(string userId, string? familyMemberId, bool? isRegular, bool? isDone)
    {
        var user = await _visitsCollection
            .Find(e => e.Id == userId)
            .FirstOrDefaultAsync();
        var medicalVisits = user.MedicalVisits;

        if (isRegular is true)
        {
            medicalVisits = sortOutRegularVisits(medicalVisits);
        }
        return medicalVisits;
    }

    public List<MedicalVisit> sortOutRegularVisits (List<MedicalVisit> visits)
    {
        List<MedicalVisit> filteredVisits = new List<MedicalVisit>();
        
        if (visits != null && visits.Count() > 0)
        {
            foreach (var visit in visits)
            {
                if (visit.IsRegular)
                {
                    filteredVisits.Add(visit);
                }
            }
        }

        return filteredVisits;
    }

}