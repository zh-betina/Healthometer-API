using System.Collections.ObjectModel;
using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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

    public async Task<string> PatchAsync(string userId, string medicalVisitId)
    {
        var filter = Builders<User>.Filter;
        var userIdAndVisitFilter = filter.And(
            filter.Eq(user => user.Id, userId),
            filter.ElemMatch(user => user.MedicalVisits, visit => visit.Id == medicalVisitId));
        var user = await _visitsCollection.Find(userIdAndVisitFilter).SingleOrDefaultAsync();

        var update = Builders<User>.Update;
        var isDoneSetter = update.Set("medical_visits.$.isDone", true);
        await _visitsCollection.UpdateOneAsync(userIdAndVisitFilter, isDoneSetter);
        
        return "Ok";
    }
}