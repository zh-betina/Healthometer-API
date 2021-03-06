using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using File = Healthometer_API.Models.DocFile;

namespace Healthometer_API.Services;

public class FileService
{
    private readonly IMongoCollection<User> _documentsCollection;
    private readonly DocumentsService _documentsService;
    public readonly long MaxSize = 3000000;
    private string _user;
    
    public FileService(
        IOptions<UsersDatabaseSettings> usersDatabaseSettings, DocumentsService documentsService)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);
        
        _documentsCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
        
        _documentsService = documentsService;
    }

    private void CreateDirectoryForUser()
    {
        var allUsersDir = Path.Combine(Directory.GetCurrentDirectory(), "Docs");
        var userDir = Path.Combine(allUsersDir, _user);

        if (Directory.Exists(userDir)) return;
        Directory.CreateDirectory(userDir);
    }

    private async Task<bool> UpdateTakenSpace(string operation, long size)
    {
        var user = await _documentsCollection.Find(user => user.Id == _user).FirstOrDefaultAsync();
        var potentialSpaceToTake = size + user.TakenSpace;

        if (MaxSize > size && MaxSize > potentialSpaceToTake)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, _user);
            var update = Builders<User>.Update.Set(field => field.TakenSpace, potentialSpaceToTake);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return true;
        }

        return false;
    }
    public async Task<Object> OnPostUploadAsync(string userId, IFormFile docFile)
    {
        _user = userId;
        CreateDirectoryForUser();
        
        var randomName = Path.GetRandomFileName();
        var folderName = Path.Combine("Docs", userId, randomName);
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        var fileExtension = Path.GetExtension(docFile.FileName);
        long fileSize = docFile.Length;

        //TODO refactor: should rather first check if size will be ok and then try to insert the file
        if (docFile.Length > 0)
        {
            var takenSpaceUpdateResult = await UpdateTakenSpace("sth", fileSize);
            if (takenSpaceUpdateResult)
            {
                await using var stream = new FileStream(pathToSave, FileMode.Create);
                docFile.CopyTo(stream);

                UpdateTakenSpace("sth", fileSize);
                var response = new {format = fileExtension, path = pathToSave};
                return response;
            }

            return "Not enough space";

        }
        return "Not ok";
    }
}