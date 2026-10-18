
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWeb.Models.Shop;
using ShopWeb.Services;
using System.Security.Claims;

namespace ShopWeb.Controllers;

[Authorize]
public class PostsController : Controller
{
    private readonly IShopApiClient _api;

    public PostsController(IShopApiClient api)
    {
        _api = api;
    }

    // GET: /Posts
    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var posts = await _api.GetPostsAsync(ct);
        return View(posts);
    }

    // GET: /Posts/Details/1
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(
        int id, CancellationToken ct)
    {
        var post = await _api.GetPostAsync(id, ct);

        if (post == null)
            return NotFound();

        var comments = await _api.GetCommentsAsync(id, ct);

        var model = new PostDetailsViewModel
        {
            Post = post,
            Comments = comments,
            NewComment = new CreateCommentRequest
            {
                PostId = id
            }
        };

        return View(model);
    }

    // GET: /Posts/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePostRequest());
    }

    // POST: /Posts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreatePostRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(request);

        await _api.CreatePostAsync(request, ct);

        return RedirectToAction(nameof(Index));
    }

    // GET: /Posts/Edit/1
    [HttpGet]
    public async Task<IActionResult> Edit(
        int id, CancellationToken ct)
    {
        var post = await _api.GetPostAsync(id, ct);

        if (post == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (post.UserId != userId)
            return Forbid();

        var request = new CreatePostRequest
        {
            Title = post.Title,
            Content = post.Content
        };

        return View(request);
    }

    // POST: /Posts/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, CreatePostRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(request);

        await _api.UpdatePostAsync(id, request, ct);

        return RedirectToAction(nameof(Index));
    }

    // POST: /Posts/Delete/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id, CancellationToken ct)
    {
        await _api.DeletePostAsync(id, ct);

        return RedirectToAction(nameof(Index));
    }
}
