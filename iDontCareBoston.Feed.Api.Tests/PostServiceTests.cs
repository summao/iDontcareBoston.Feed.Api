using Xunit;
using Moq;
using iDontCareBoston.Feed.Api.Services;
using iDontCareBoston.Feed.Api.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq; // Required for ToListAsync() on IAsyncCursor if not directly available as a method

namespace iDontCareBoston.Feed.Api.Tests.Services
{
    public class PostServiceTests
    {
        private readonly Mock<IMongoCollection<Post>> _mockPostCollection;
        private readonly Mock<IMongoClient> _mockMongoClient;
        private readonly Mock<IMongoDatabase> _mockMongoDatabase;
        private readonly PostService _postService;

        public PostServiceTests()
        {
            _mockPostCollection = new Mock<IMongoCollection<Post>>();
            _mockMongoClient = new Mock<IMongoClient>();
            _mockMongoDatabase = new Mock<IMongoDatabase>();

            // Setup IMongoClient to return the mock IMongoDatabase
            _mockMongoClient.Setup(client => client.GetDatabase("Feed", null))
                .Returns(_mockMongoDatabase.Object);

            // Setup IMongoDatabase to return the mock IMongoCollection
            _mockMongoDatabase.Setup(db => db.GetCollection<Post>("Posts", null))
                .Returns(_mockPostCollection.Object);

            _postService = new PostService(_mockMongoClient.Object);
        }

        [Fact]
        public async Task GetPosts_WhenNoPostsExist_ReturnsEmptyList()
        {
            // Arrange
            var emptyPostsCursor = new Mock<IAsyncCursor<Post>>();
            emptyPostsCursor.Setup(_ => _.Current).Returns(new List<Post>()); // Current should be an empty list initially or after MoveNextAsync if no elements
            emptyPostsCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true) // First call to MoveNextAsync to indicate some data (empty batch)
                .ReturnsAsync(false); // Subsequent calls indicate no more data

            _mockPostCollection.Setup(col => col.FindAsync(
                It.IsAny<FilterDefinition<Post>>(),
                It.IsAny<FindOptions<Post, Post>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(emptyPostsCursor.Object);
            
            // Act
            var result = await _postService.GetPosts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPosts_WhenPostsExist_ReturnsListOfPosts()
        {
            // Arrange
            var expectedPosts = new List<Post>
            {
                new Post { Id = "1", Message = "Test Post 1", CreatedDateTime = System.DateTime.UtcNow, Author = new Author { Username = "user1", DisplayName = "User One", ProfileImagePath = "path/to/image1.jpg" } },
                new Post { Id = "2", Message = "Test Post 2", CreatedDateTime = System.DateTime.UtcNow, Author = new Author { Username = "user2", DisplayName = "User Two", ProfileImagePath = "path/to/image2.jpg" } }
            };

            var mockCursor = new Mock<IAsyncCursor<Post>>();
            mockCursor.Setup(_ => _.Current).Returns(expectedPosts);
            mockCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true) // Indicates that there is data in Current
                .ReturnsAsync(false); // Indicates no more data

            _mockPostCollection.Setup(col => col.FindAsync(
                It.IsAny<FilterDefinition<Post>>(),
                It.IsAny<FindOptions<Post, Post>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _postService.GetPosts();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPosts.Count, result.Count);
            Assert.Equal(expectedPosts[0].Message, result[0].Message);
            Assert.Equal(expectedPosts[1].Message, result[1].Message);
        }

        [Fact]
        public async Task AddPost_GivenMessage_CallsInsertOneAsyncWithCorrectPost()
        {
            // Arrange
            var testMessage = "Hello Test Post!";
            Post capturedPost = null;

            _mockPostCollection.Setup(col => col.InsertOneAsync(
                It.IsAny<Post>(),
                null, // InsertOneOptions can be null
                It.IsAny<CancellationToken>()
            )).Callback<Post, InsertOneOptions, CancellationToken>((post, options, token) => {
                capturedPost = post;
            }).Returns(Task.CompletedTask);

            // Act
            await _postService.AddPost(testMessage);

            // Assert
            _mockPostCollection.Verify(col => col.InsertOneAsync(
                It.IsAny<Post>(),
                null,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            Assert.NotNull(capturedPost);
            Assert.Equal(testMessage, capturedPost.Message);
            Assert.True((System.DateTime.UtcNow - capturedPost.CreatedDateTime).TotalSeconds < 5, "CreatedDateTime should be recent.");
            Assert.NotNull(capturedPost.Author);
            Assert.Equal("@namking", capturedPost.Author.Username);
            Assert.Equal("น้องน้ำขิง ไม่จิงกะเบล", capturedPost.Author.DisplayName);
            // Assuming ProfileImagePath is empty string as per PostService
            Assert.Equal("", capturedPost.Author.ProfileImagePath);
        }
    }
}
