using Moq;
using iDontCareBoston.Feed.Api.Services;
using iDontCareBoston.Feed.Api.Entities;
using iDontCareBoston.Feed.Api.Repositories;

namespace iDontCareBoston.Feed.Api.Tests.Services;

public class PostServiceTests
{
    private readonly PostService _postService;

    private readonly Mock<PostRepository> _mockPostRepository;
    private readonly Mock<TimeProvider> _mockTimeProvider;

    public PostServiceTests()
    {
        _mockPostRepository = new();
        _mockTimeProvider = new();

        _postService = new PostService(_mockPostRepository.Object, _mockTimeProvider.Object);
    }

    // [Fact]
    // public async Task GetPosts_WhenNoPostsExist_ReturnsEmptyList()
    // {
    //     // Arrange
    //     var emptyPostsCursor = new Mock<IAsyncCursor<Post>>();
    //     emptyPostsCursor.Setup(_ => _.Current).Returns(new List<Post>());
    //     emptyPostsCursor
    //         .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(true)
    //         .ReturnsAsync(false);

    //     _mockPostCollection.Setup(col => col.FindAsync(
    //         It.IsAny<FilterDefinition<Post>>(),
    //         It.IsAny<FindOptions<Post, Post>>(),
    //         It.IsAny<CancellationToken>()
    //     )).ReturnsAsync(emptyPostsCursor.Object);

    //     // Act
    //     var result = await _postService.GetPosts();

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Empty(result);
    // }

    // [Fact]
    // public async Task GetPosts_WhenPostsExist_ReturnsListOfPosts()
    // {
    //     // Arrange
    //     var expectedPosts = new List<Post>
    //         {
    //             new() { Id = "1", Message = "Test Post 1", CreatedDateTime = System.DateTime.UtcNow, Author = new Author { Username = "user1", DisplayName = "User One", ProfileImagePath = "path/to/image1.jpg" }, Visibility = PostVisibility.Private },
    //             new() { Id = "2", Message = "Test Post 2", CreatedDateTime = System.DateTime.UtcNow, Author = new Author { Username = "user2", DisplayName = "User Two", ProfileImagePath = "path/to/image2.jpg" }, Visibility = PostVisibility.Private }
    //         };

    //     var mockCursor = new Mock<IAsyncCursor<Post>>();
    //     mockCursor.Setup(_ => _.Current).Returns(expectedPosts);
    //     mockCursor
    //         .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(true)
    //         .ReturnsAsync(false);

    //     _mockPostCollection.Setup(col => col.FindAsync(
    //         It.IsAny<FilterDefinition<Post>>(),
    //         It.IsAny<FindOptions<Post, Post>>(),
    //         It.IsAny<CancellationToken>()
    //     )).ReturnsAsync(mockCursor.Object);

    //     // Act
    //     var result = await _postService.GetPosts();

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(expectedPosts.Count, result.Count);
    //     Assert.Equal(expectedPosts[0].Message, result[0].Message);
    //     Assert.Equal(expectedPosts[1].Message, result[1].Message);
    // }

    [Fact]
    public async Task AddPost_ShouldCallRepositoryAdd_WithCorrectPost()
    {
        // Arrange
        string message = "message";
        var visibility = PostVisibility.Public;
        var now = DateTimeOffset.Now;
        _mockTimeProvider.Setup(a => a.GetUtcNow())
            .Returns(now);

        Post? capturedPost = null;
        _mockPostRepository.Setup(r => r.Add(It.IsAny<Post>()))
            .Callback<Post>(p => capturedPost = p)
            .Returns(Task.CompletedTask);

        // Act
        await _postService.AddPost(message, visibility);

        // Assert
        _mockPostRepository.Verify(r => r.Add(It.IsAny<Post>()), Times.Once);
        Assert.NotNull(capturedPost);
        Assert.Equal(message, capturedPost.Message);
        Assert.Equal(visibility, capturedPost.Visibility);
        Assert.NotNull(capturedPost.Author);
        Assert.Equal("@namking", capturedPost.Author.Username);
        Assert.Equal("น้องน้ำขิง ไม่จิงกะเบล", capturedPost.Author.DisplayName);
        Assert.Equal("", capturedPost.Author.ProfileImagePath);
        Assert.Equal(now, capturedPost.CreatedDateTime);
    }
}
