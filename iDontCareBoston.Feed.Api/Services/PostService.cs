using iDontCareBoston.Feed.Api.Entities;
using MongoDB.Driver;

namespace iDontCareBoston.Feed.Api.Services;

public class PostService
{
    private readonly IMongoCollection<Post> col;
    public PostService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("Feed");
        col = database.GetCollection<Post>("Posts");
    }

    public virtual async Task<List<Post>> GetPosts()
    {
        return await col.Find(_ => true).ToListAsync();
    }

    public virtual async Task AddPost(string message)
    {
        var newPost = new Post
        {
            Message = message,
            CreatedDateTime = DateTime.Now,
            Author = new Author
            {
                //TODO get id from token แล้วดึงข้อมูลจาก member ที่ consume มา
                Username = "@namking",
                DisplayName = "น้องน้ำขิง ไม่จิงกะเบล",
                ProfileImagePath = ""
            }
        };
        await col.InsertOneAsync(newPost);
    }
}