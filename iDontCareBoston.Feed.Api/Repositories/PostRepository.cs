using iDontCareBoston.Feed.Api.Entities;
using MongoDB.Driver;

namespace iDontCareBoston.Feed.Api.Repositories;

// public interface IPostRepository
// {
//     Task<List<Post>> Get(int skip = 0, int limit = 10, bool isAscending = false);
//     Task Add(Post post);
// }

public class PostRepository(MongoDbContext context)
{
    private readonly IMongoCollection<Post> _posts = context.Posts;

    public virtual async Task<List<Post>> Get(int skip = 0, int limit = 10, bool isAscending = false)
    {
        var find = _posts.Find(_ => true);
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

    public async Task Add(Post post)
    {
        await _posts.InsertOneAsync(post);
    }
}
