using System.Net.Http.Headers;
using Healthometer_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using File = Healthometer_API.Models.DocFile;

namespace Healthometer_API.Services;

public class UploadFileService
{
    private readonly IMongoCollection<User> _documentsCollection;

    public UploadFileService(
        IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _documentsCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
    }

    public void CreateDirectoryForUser(string userId)
    {
        var allUsersDir = Path.Combine(Directory.GetCurrentDirectory(), "Docs");
        var userDir = Path.Combine(allUsersDir, userId);

        if (Directory.Exists(userDir)) return;
        Directory.CreateDirectory(userDir);
    }

    public async Task<string> OnPostUploadAsync(DocFile file)
    {
        var docInfo = file.Doc;
        var docFile = file.FileContent;
        Console.WriteLine("TEST");
        var folderName = Path.Combine("Docs", "TestUser");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        if (docFile.Length <= 0) return "not Ok";
        var fileName = ContentDispositionHeaderValue.Parse(docFile.ContentDisposition).FileName.Trim('"');
        var fullPath = Path.Combine(pathToSave, fileName);
        var dbPath = Path.Combine(folderName, fileName);
        Console.WriteLine(docInfo);
        await using var stream = new FileStream(fullPath, FileMode.Create);
        docFile.CopyTo(stream);
        return "Ok";
    }
}