using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Healthometer_API.Services;

public class FamilyMemberService
{
    private readonly IMongoCollection<User> _familyMembersCollection;
    private readonly FileService _fileService;
    private string _family;


    public FamilyMemberService(
        IOptions<UsersDatabaseSettings> usersDatabaseSettings, FileService fileService)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _familyMembersCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
        
        _fileService = fileService;

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

    public async Task<string> PostDocAsync(string userId, string familyId, DocFile docFile)
    {
        
        _fileService.CreateDirectoryForUser(familyId);

        IFormFile file = docFile.FileContent;
        var docInfo = docFile.DocumentInfo;
        var randomName = Path.GetRandomFileName();
        var folderName = Path.Combine("Docs", userId, randomName);
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        var fileExtension = Path.GetExtension(file.FileName);
        long fileSize = file.Length;

        Document doc = new Document();
        //TODO refactor: should rather first check if size will be ok and then try to insert the file
        if (file.Length > 0)
        {
            var takenSpaceUpdateResult = await _fileService.UpdateTakenSpace(userId, "sth", fileSize);
            if (takenSpaceUpdateResult)
            {
                await using var stream = new FileStream(pathToSave, FileMode.Create);
                file.CopyTo(stream);

                _fileService.UpdateTakenSpace(userId, "sth", fileSize);
                doc = new Document
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Format = fileExtension,
                    Path = pathToSave,
                    Date = docInfo?.Date,
                    Category = docInfo?.Category,
                    Name = docInfo?.Name,
                    Status = docInfo?.Status

                };
            }
        }

        var filter = Builders<User>.Filter.Eq(u => u.Id, userId) &
                     Builders<User>.Filter.ElemMatch(f => f.FamilyMembers,
                         Builders<FamilyMember>.Filter.Eq(f => f.Id, familyId));
        var update = Builders<User>.Update.Push("family.$.docs", doc);

        var result = await _familyMembersCollection.FindOneAndUpdateAsync(filter, update);
        
        if (result is not null)
        {
            Console.WriteLine(result);
            return "ok";
        }

        return "not ok";
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