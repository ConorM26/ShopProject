
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWeb.Models.Shop;
using ShopWeb.Services;
using System.Security.Claims;

namespace ShopWeb.Controllers;

[Authorize]
public class CommentsController : Controller
{
    private readonly IShopApiClient _api;

    public CommentsController(IShopApiClient api)
    {
        _api = api;
    }

    // POST: /Comments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateCommentRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please enter a valid comment.");

        await _api.CreateCommentAsync(request, ct);

        return RedirectToAction(
            "Details",
            "Posts",
            new { id = request.PostId });
    }

    // GET: /Comments/Edit/1
    [HttpGet]
    public async Task<IActionResult> Edit(
        int postId, int id, CancellationToken ct)
    {
        var comments = await _api.GetCommentsAsync(postId, ct);
        var comment = comments.FirstOrDefault(c => c.Id == id);

        if (comment == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (comment.UserId != userId)
            return Forbid();

        ViewBag.PostId = postId;

        return View(comment);
    }

    // POST: /Comments/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, int postId, string content, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(content) || content.Length > 500)
            return BadRequest("Please enter a valid comment.");

        await _api.UpdateCommentAsync(id, content, ct);

        return RedirectToAction(
            "Details",
            "Posts",
            new { id = postId });
    }

    // POST: /Comments/Delete/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id, int postId, CancellationToken ct)
    {
        await _api.DeleteCommentAsync(id, ct);

        return RedirectToAction(
            "Details",
            "Posts",
            new { id = postId });
    }
}
