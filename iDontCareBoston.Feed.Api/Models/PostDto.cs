using iDontCareBoston.Feed.Api.Entities;

namespace iDontCareBoston.Feed.Api;

public class PostDto
{
    public required string Id { get; set; }
    public required string Message { get; set; }
    public required Author Author { get; set; }
    public required DateTimeOffset CreatedDateTime { get; set; }
    public required string Visibility { get; set; }
}