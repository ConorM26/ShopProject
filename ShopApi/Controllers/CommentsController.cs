
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ShopContext _context;

    public CommentsController(ShopContext context)
    {
        _context = context;
    }

    // GET: /api/comments/post/1
    [HttpGet("post/{postId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetComments(int postId)
    {
        var comments = await _context.Comments
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return Ok(comments);
    }


    // POST: /api/comments
    [HttpPost]
    public async Task<IActionResult> CreateComment(
        [FromBody] CreateCommentRequest request)
    {
        var userId = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var postExists = await _context.Posts
            .AnyAsync(p => p.Id == request.PostId);

        if (!postExists)
            return NotFound("Post not found.");

        var comment = new Comment
        {
            PostId = request.PostId,
            Content = request.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }


    // PUT: /api/comments/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(
        int id, [FromBody] UpdateCommentRequest request)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
            return NotFound();

        var userId = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (comment.UserId != userId)
            return Forbid();

        comment.Content = request.Content;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    // DELETE: /api/comments/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment == null)
            return NotFound();

        var userId = User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (comment.UserId != userId)
            return Forbid();

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync();

        return NoContent();
    }


}

public class CreateCommentRequest
{
    [Required]
    public int PostId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
}


public class UpdateCommentRequest
{
    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
}