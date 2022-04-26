using Healthometer_API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Healthometer_API.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersService(
        IOptions<UsersDatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<User>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<int> CreateAsync(User newUser)
    {
        var condition = newUser.Name is not null & newUser.Email is not null & newUser.Password is not null;
        if (condition)
        {
            await _usersCollection.InsertOneAsync(newUser);
            return 1;
        }

        return 0;
    }
    
    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
    
}