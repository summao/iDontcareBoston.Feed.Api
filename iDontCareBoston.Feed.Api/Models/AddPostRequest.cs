namespace iDontCareBoston.Feed.Api.Models;

public class AddPostRequest
{
    public required string PostMessage { get; set; }
    public required string PostVisibility { get; set; }
}