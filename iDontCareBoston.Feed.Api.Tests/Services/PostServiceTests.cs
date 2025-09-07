using Moq;
using iDontCareBoston.Feed.Api.Services;
using iDontCareBoston.Feed.Api.Entities;
using iDontCareBoston.Feed.Api.Repositories;

namespace iDontCareBoston.Feed.Api.Tests.Services;

public class PostServiceTests
{
    private readonly PostService _postService;

    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly Mock<TimeProvider> _mockTimeProvider;

    public PostServiceTests()
    {
        _mockPostRepository = new();
        _mockTimeProvider = new();

        _postService = new PostService(_mockPostRepository.Object, _mockTimeProvider.Object);
    }

    #region GetPosts
    [Fact]
    public async Task GetPosts_CallRepo()
    {
        // Arrange

        // Act
        await _postService.GetPosts();

        // Assert
        _mockPostRepository.Verify(a => a.GetMany(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()));
    }
    #endregion

    #region AddPost
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
    #endregion
}
