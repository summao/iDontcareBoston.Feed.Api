namespace iDontCareBoston.Feed.Api.Models;

public class FeedDatabaseSetting
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string PostsCollectionName { get; set; } = null!;
}