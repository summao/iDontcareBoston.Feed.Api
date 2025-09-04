// using Moq;
// using iDontCareBoston.Feed.Api.Controllers;
// using iDontCareBoston.Feed.Api.Services;
// using iDontCareBoston.Feed.Api.Entities;
// using iDontCareBoston.Feed.Api.Models;
// using Microsoft.AspNetCore.Mvc;
// using MongoDB.Driver;

// namespace iDontCareBoston.Feed.Api.Tests.Controllers;

// public class PostsControllerTests
// {
//     private readonly Mock<PostService> _mockPostService;
//     private readonly PostsController _postsController;

//     public PostsControllerTests()
//     {
//         _mockPostService = new Mock<PostService>(Mock.Of<IMongoClient>());
//         _postsController = new PostsController(_mockPostService.Object);
//     }

//     [Fact]
//     public async Task GetPosts_WhenServiceReturnsPosts_ReturnsOkObjectResultWithPosts()
//     {
//         // Arrange
//         var expectedPosts = new List<Post>
//             {
//                 new() { Id = "1", Message = "Controller Test Post 1", Author = new Author { Username = "test", DisplayName = "Test User", ProfileImagePath = "" }, CreatedDateTime = System.DateTime.UtcNow, Visibility = PostVisibility.Private },
//                 new() { Id = "2", Message = "Controller Test Post 2", Author = new Author { Username = "test2", DisplayName = "Test User 2", ProfileImagePath = "" }, CreatedDateTime = System.DateTime.UtcNow, Visibility = PostVisibility.Private }
//             };
//         _mockPostService.Setup(service => service.GetPosts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
//             .ReturnsAsync(expectedPosts);

//         // Act
//         var result = await _postsController.GetPosts();

//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         var actualPosts = Assert.IsAssignableFrom<IEnumerable<Post>>(okResult.Value);
//         Assert.Equal(expectedPosts.Count, new List<Post>(actualPosts).Count);
//     }

//     [Fact]
//     public async Task GetPosts_WhenServiceReturnsEmptyList_ReturnsOkObjectResultWithEmptyList()
//     {
//         // Arrange
//         var emptyPosts = new List<Post>();
//         _mockPostService.Setup(service => service.GetPosts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
//             .ReturnsAsync(emptyPosts);

//         // Act
//         var result = await _postsController.GetPosts();

//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         var actualPosts = Assert.IsAssignableFrom<IEnumerable<Post>>(okResult.Value);
//         Assert.Empty(actualPosts);
//     }

//     [Fact]
//     public async Task AddPost_ValidRequest_CallsServiceAndReturnsOkResult()
//     {
//         // Arrange
//         var request = new AddPostRequest { PostMessage = "New controller post", PostVisibility = "private" };
//         _mockPostService.Setup(service => service.AddPost(request.PostMessage, It.IsAny<PostVisibility>()))
//             .Returns(Task.CompletedTask);

//         // Act
//         var result = await _postsController.AddPost(request);

//         // Assert
//         Assert.IsType<OkResult>(result);
//         _mockPostService.Verify(service => service.AddPost(request.PostMessage, It.IsAny<PostVisibility>()), Times.Once);
//     }

//     [Fact]
//     public async Task AddPost_ServiceCalledWithCorrectMessage()
//     {
//         // Arrange
//         var testMessage = "Verify this message";
//         var request = new AddPostRequest { PostMessage = testMessage, PostVisibility = "private" };
//         var capturedMessage = "";

//         _mockPostService.Setup(service => service.AddPost(It.IsAny<string>(), It.IsAny<PostVisibility>()))
//             .Callback<string>(message => capturedMessage = message)
//             .Returns(Task.CompletedTask);

//         // Act
//         await _postsController.AddPost(request);

//         // Assert
//         _mockPostService.Verify(service => service.AddPost(testMessage, It.IsAny<PostVisibility>()), Times.Once);
//         Assert.Equal(testMessage, capturedMessage);
//     }
// }
