using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace iDontCareBoston.Feed.Api.Entities;

public class Post
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string Message { get; set; }
    public required Author Author { get; set; }
    public required DateTime CreatedDateTime { get; set; }
}

public class Author
{
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public required string ProfileImagePath { get; set; }
}