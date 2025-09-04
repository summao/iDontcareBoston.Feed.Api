using iDontCareBoston.Feed.Api.Entities;
using MongoDB.Driver;

namespace iDontCareBoston.Feed.Api;

public class MongoDbContext(IMongoClient mongoClient)
{
    private readonly IMongoDatabase _database = mongoClient.GetDatabase("Feed");

    public IMongoCollection<Post> Posts => _database.GetCollection<Post>("Posts");
}
