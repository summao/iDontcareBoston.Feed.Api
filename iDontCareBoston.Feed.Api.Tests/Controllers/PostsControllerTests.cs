using Moq;
using iDontCareBoston.Feed.Api.Controllers;
using iDontCareBoston.Feed.Api.Services;
using iDontCareBoston.Feed.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using iDontCareBoston.Feed.Api.Models;

namespace iDontCareBoston.Feed.Api.Tests.Controllers;

public class PostsControllerTests
{
    private readonly Mock<IPostService> _mockPostService;
    private readonly PostsController _postsController;

    public PostsControllerTests()
    {
        _mockPostService = new();
        _postsController = new PostsController(_mockPostService.Object);
    }

    #region GetPosts
    [Fact]
    public async Task GetPosts_CallService()
    {
        // Arrange

        // Act
        await _postsController.GetPosts();

        // Assert
        _mockPostService.Verify(a => a.GetPosts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()));
    }
    #endregion

    #region AddPost
    [Fact]
    public async Task AddPost_ValidRequest_CallsService()
    {
        // Arrange
        var request = new AddPostRequest { PostMessage = "PostMessage", PostVisibility = "private" };

        // Act
        await _postsController.AddPost(request);

        // Assert
        _mockPostService.Verify(service => service.AddPost(request.PostMessage, It.IsAny<PostVisibility>()));
    }

    [Fact]
    public async Task AddPost_ReturnBadRequest_WhenWrongPostVisibility()
    {
        // Arrange
        var request = new AddPostRequest { PostMessage = "PostMessage", PostVisibility = "not private" };

        // Act
        var result = await _postsController.AddPost(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion
}
