using Healthometer_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;

namespace Healthometer_API.Services;

public class CategoriesService
{
    private readonly IMongoCollection<User> _usersCollection;

    public CategoriesService(
        IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);
        
        _usersCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<string>>? GetAsync(string userId)
    {
        var user = await _usersCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
        return user.Categories;
    }

    public async Task<string>? PostAsync(string userId, string category)
    {
        var categoryLowCase = category.ToLower();
        var user = await _usersCollection
            .Find<User>(user => user.Id == userId)
            .FirstOrDefaultAsync();

        var existingCategories = user.Categories;

        if (existingCategories.Contains(categoryLowCase))
        {
            return "exists";
        }

        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId));
        var update = Builders<User>.Update.Push("categories", categoryLowCase);
        await _usersCollection.UpdateOneAsync(filter, update);
        return "ok";
    }

    public async Task<string> DeleteAsync(string userId, string category)
    {
        var categoryLowCase = category.ToLower();
        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId));
        var update = Builders<User>.Update.Pull("categories", categoryLowCase);
        await _usersCollection.UpdateOneAsync(filter, update);
        return "ok";
    }

    public async Task<string> ModifyAsync(string userId, string oldCategory, string newCategory)
    {
        var filter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId));

        await DeleteAsync(userId, oldCategory);
        await PostAsync(userId, newCategory);
        
        var newCategLowcase = oldCategory.ToLower();
        var oldCategLowcase = newCategory.ToLower();
        
        //TODO update all categories already used in documents (docs)
        return "ok";
    }
}