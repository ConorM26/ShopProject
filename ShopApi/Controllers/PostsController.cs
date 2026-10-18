
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly ShopContext _context;

    public PostsController(ShopContext context)
    {
        _context = context;
    }

    // GET - api/posts
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(posts);
    }

    // GET - api/posts/1
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPost(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            return NotFound();

        return Ok(post);
    }

    // POST - api/posts
    [HttpPost]
    public async Task<IActionResult> CreatePost(
        [FromBody] CreatePostRequest request)
    {
        var userId = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPost),
            new { id = post.Id },
            post);
    }

    // PUT - api/posts/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(
        int id, [FromBody] CreatePostRequest request)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            return NotFound();

        var userId = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (post.UserId != userId)
            return Forbid();

        post.Title = request.Title;
        post.Content = request.Content;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE - api/posts/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
            return NotFound();

        var userId = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (post.UserId != userId)
            return Forbid();

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreatePostRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}
