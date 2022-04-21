using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using File = Healthometer_API.Models.DocFile;

namespace Healthometer_API.Services;

public class FileService
{
    private readonly IMongoCollection<User> _documentsCollection;
    private readonly DocumentsService _documentsService;
    
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

    private void CreateDirectoryForUser(string userId)
    {
        var allUsersDir = Path.Combine(Directory.GetCurrentDirectory(), "Docs");
        var userDir = Path.Combine(allUsersDir, userId);

        if (Directory.Exists(userDir)) return;
        Directory.CreateDirectory(userDir);
    }

    private Document PrepareDocInfoForDatabase(Document docInfo, string format, string path)
    {
        var docToSendToDb = new Document
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Format = format,
            Path = path,
            Date = docInfo.Date,
            Category = docInfo.Category,
            Name = docInfo.Name,
            Status = docInfo.Status
        };

        return docToSendToDb;
    }
    public async Task<string> OnPostUploadAsync(string userId, IFormFile docFile, Document docInfo)
    {
        CreateDirectoryForUser(userId);
        
        var randomName = Path.GetRandomFileName();
        var folderName = Path.Combine("Docs", userId, randomName);
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        var fileExtension = Path.GetExtension(docFile.FileName);
        
        if (docFile.Length > 0)
        {
            await using var stream = new FileStream(pathToSave, FileMode.Create);
            docFile.CopyTo(stream);

            var docInfoForDatabase = PrepareDocInfoForDatabase(docInfo, fileExtension, pathToSave);
            
            await _documentsService.PutAsync(userId, docInfoForDatabase);
        
            return "ok";
        }
        return "Not ok";
    }
}