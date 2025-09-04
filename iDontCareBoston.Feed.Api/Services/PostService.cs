using iDontCareBoston.Feed.Api.Entities;
using iDontCareBoston.Feed.Api.Repositories;

namespace iDontCareBoston.Feed.Api.Services;

public class PostService(PostRepository _postRepository, TimeProvider _timeProvider)
{

    // public virtual async Task<List<Post>> GetPosts(int skip = 0, int limit = 10, bool isAscending = false)
    // {
    //     var find = _posts.Find(_ => true);
    //     if (isAscending)
    //     {
    //         find.SortBy(a => a.CreatedDateTime);
    //     }
    //     else
    //     {
    //         find.SortByDescending(a => a.CreatedDateTime);

    //     }
    //     return await find.Skip(skip).Limit(limit).ToListAsync();
    // }

    public virtual async Task AddPost(string message, PostVisibility postVisibility)
    {
        var newPost = new Post
        {
            Message = message,
            CreatedDateTime = _timeProvider.GetUtcNow(),
            Author = new Author
            {
                //TODO get id from token แล้วดึงข้อมูลจาก member ที่ consume มา
                Username = "@namking",
                DisplayName = "น้องน้ำขิง ไม่จิงกะเบล",
                ProfileImagePath = ""
            },
            Visibility = postVisibility,
        };
        await _postRepository.Add(newPost);
    }
}