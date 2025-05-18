using System.CodeDom.Compiler;
using iDontCareBoston.Feed.Api.Entities;
using iDontCareBoston.Feed.Api.Models;
using iDontCareBoston.Feed.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace iDontCareBoston.Feed.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController(PostService _postService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        var result = await _postService.GetPosts();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddPost(AddPostRequest addPostRequest)
    {
        await _postService.AddPost(addPostRequest.PostMessage);
        return Ok();
    }
}