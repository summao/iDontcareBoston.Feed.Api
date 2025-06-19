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

    public virtual async Task<List<Post>> GetPosts(int skip = 0, int limit = 10, bool isAscending = false)
    {
        var find = col.Find(_ => true);
        if (isAscending)
        {
            find.SortBy(a => a.CreatedDateTime);
        }
        else
        {
            find.SortByDescending(a => a.CreatedDateTime);
            
        }
        return await find.Skip(skip).Limit(limit).ToListAsync();
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