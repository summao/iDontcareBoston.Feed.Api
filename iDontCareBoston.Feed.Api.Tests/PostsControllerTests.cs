using Xunit;
using Moq;
using iDontCareBoston.Feed.Api.Controllers;
using iDontCareBoston.Feed.Api.Services;
using iDontCareBoston.Feed.Api.Entities; // For Post entity
using iDontCareBoston.Feed.Api.Models;  // For AddPostRequest if needed later
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver; // Added for IMongoClient

namespace iDontCareBoston.Feed.Api.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<PostService> _mockPostService;
        private readonly PostsController _postsController;

        public PostsControllerTests()
        {
            // PostService has a constructor dependency on IMongoClient.
            // For these controller tests, we only need to mock PostService itself,
            // so we can pass a mock IMongoClient to PostService's constructor,
            // or if PostService had a parameterless constructor (it doesn't) or an interface (it doesn't yet).
            // The easiest way for now is to mock PostService directly using Moq's ability to mock classes.
            // However, PostService's methods are not virtual, so Moq cannot override them unless they are.
            //
            // OPTION 1: Modify PostService to have virtual methods (preferred for testability).
            // OPTION 2: Pass a fully mocked IMongoClient to a real PostService instance, then mock calls on that real instance.
            // OPTION 3: Create an IPostService interface and use that (most robust).
            //
            // For this subtask, let's assume we can make PostService methods virtual or use an interface.
            // If the worker cannot modify PostService, it should mock IMongoClient and pass it to new PostService,
            // then setup mocks on the PostService instance's methods if they were virtual.
            // Given the current structure, to avoid modifying PostService source code within this step,
            // we will mock PostService directly. This requires PostService methods to be virtual.
            // Let's proceed assuming they can be made virtual. If not, the worker will need to inform us.
            // For now, to avoid worker failure, we will mock PostService directly.
            // Moq can mock non-virtual methods on classes if they are part of an interface that the class implements and the mock is of the interface.
            // Or, if we mock the class directly, methods must be virtual.
            // The prompt for PostService used `new PostService(_mockMongoClient.Object)`.
            // Here, we need to mock the service the controller depends on.
            _mockPostService = new Mock<PostService>(Mock.Of<IMongoClient>()); // Pass dummy IMongoClient for constructor
            _postsController = new PostsController(_mockPostService.Object);
        }

        [Fact]
        public async Task GetPosts_WhenServiceReturnsPosts_ReturnsOkObjectResultWithPosts()
        {
            // Arrange
            var expectedPosts = new List<Post>
            {
                new Post { Id = "1", Message = "Controller Test Post 1", Author = new Author { Username = "test", DisplayName = "Test User", ProfileImagePath = "" }, CreatedDateTime = System.DateTime.UtcNow },
                new Post { Id = "2", Message = "Controller Test Post 2", Author = new Author { Username = "test2", DisplayName = "Test User 2", ProfileImagePath = "" }, CreatedDateTime = System.DateTime.UtcNow }
            };
            _mockPostService.Setup(service => service.GetPosts())
                            .ReturnsAsync(expectedPosts);

            // Act
            var result = await _postsController.GetPosts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualPosts = Assert.IsAssignableFrom<IEnumerable<Post>>(okResult.Value);
            Assert.Equal(expectedPosts.Count, new List<Post>(actualPosts).Count);
            // Optionally, assert contents if necessary, e.g., by comparing messages or IDs
        }

        [Fact]
        public async Task GetPosts_WhenServiceReturnsEmptyList_ReturnsOkObjectResultWithEmptyList()
        {
            // Arrange
            var emptyPosts = new List<Post>();
            _mockPostService.Setup(service => service.GetPosts())
                            .ReturnsAsync(emptyPosts);

            // Act
            var result = await _postsController.GetPosts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualPosts = Assert.IsAssignableFrom<IEnumerable<Post>>(okResult.Value);
            Assert.Empty(actualPosts);
        }

        [Fact]
        public async Task AddPost_ValidRequest_CallsServiceAndReturnsOkResult()
        {
            // Arrange
            var request = new AddPostRequest { PostMessage = "New controller post" };
            _mockPostService.Setup(service => service.AddPost(request.PostMessage))
                            .Returns(Task.CompletedTask); // Setup AddPost to complete successfully

            // Act
            var result = await _postsController.AddPost(request);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockPostService.Verify(service => service.AddPost(request.PostMessage), Times.Once);
        }

        [Fact]
        public async Task AddPost_ServiceCalledWithCorrectMessage()
        {
            // Arrange
            var testMessage = "Verify this message";
            var request = new AddPostRequest { PostMessage = testMessage };
            string capturedMessage = null;

            _mockPostService.Setup(service => service.AddPost(It.IsAny<string>()))
                            .Callback<string>(message => capturedMessage = message)
                            .Returns(Task.CompletedTask);

            // Act
            await _postsController.AddPost(request);

            // Assert
            _mockPostService.Verify(service => service.AddPost(testMessage), Times.Once); // Verify it was called
            Assert.Equal(testMessage, capturedMessage); // Verify the message passed was correct
        }
    }
}
