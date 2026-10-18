
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ShopWeb.Models.Shop;

namespace ShopWeb.Services;

public class ShopApiClient(HttpClient http) : IShopApiClient
{
    // ITEMS

    public async Task<IReadOnlyList<ItemDto>> GetItemsAsync(
        CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<ItemDto>>("api/items", ct) ?? [];

    public Task<ItemDto?> GetItemAsync(
        int id, CancellationToken ct = default)
        => GetOrNullAsync<ItemDto>($"api/items/{id}", ct);


    // ORDERS

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(
        CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<OrderDto>>("api/orders", ct) ?? [];

    public Task<OrderDto?> GetOrderAsync(
        int id, CancellationToken ct = default)
        => GetOrNullAsync<OrderDto>($"api/orders/{id}", ct);

    public async Task<CreateOrderResult> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync(
            "api/orders", request, ct);

        // 400 = the API rejected the order
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problem = await response.Content
                .ReadFromJsonAsync<ValidationProblemDetails>(ct);

            var errors = problem?.Errors
                .SelectMany(e => e.Value)
                .ToList()
                ?? ["The shop rejected the order."];

            return new CreateOrderResult(null, errors);
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content
            .ReadFromJsonAsync<OrderDto>(ct)
            ?? throw new InvalidOperationException(
                "The API returned an empty order.");

        return new CreateOrderResult(order, []);
    }


    // POSTS

    public async Task<IReadOnlyList<PostDto>> GetPostsAsync(
        CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<PostDto>>("api/posts", ct) ?? [];

    public Task<PostDto?> GetPostAsync(
        int id, CancellationToken ct = default)
        => GetOrNullAsync<PostDto>($"api/posts/{id}", ct);

    public async Task<PostDto> CreatePostAsync(
        CreatePostRequest request,
        CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync(
            "api/posts", request, ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PostDto>(ct)
            ?? throw new InvalidOperationException(
                "The API returned an empty post.");
    }

    public async Task UpdatePostAsync(
        int id, CreatePostRequest request,
        CancellationToken ct = default)
    {
        using var response = await http.PutAsJsonAsync(
            $"api/posts/{id}", request, ct);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeletePostAsync(
        int id, CancellationToken ct = default)
    {
        using var response = await http.DeleteAsync(
            $"api/posts/{id}", ct);

        response.EnsureSuccessStatusCode();
    }


    // GET: api/comments/post/1
    public async Task<IReadOnlyList<CommentDto>> GetCommentsAsync(
        int postId, CancellationToken ct = default)
    {
        var comments = await http.GetFromJsonAsync<List<CommentDto>>(
            $"api/comments/post/{postId}", ct);

        return comments ?? [];
    }

    // POST: api/comments
    public async Task<CommentDto> CreateCommentAsync(
        CreateCommentRequest request, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync(
            "api/comments", request, ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CommentDto>(ct)
            ?? throw new InvalidOperationException(
                "The API returned an empty comment.");
    }


    // PUT: api/comments/1
    public async Task UpdateCommentAsync(
        int id, string content, CancellationToken ct = default)
    {
        var request = new { Content = content };

        using var response = await http.PutAsJsonAsync(
            $"api/comments/{id}", request, ct);

        response.EnsureSuccessStatusCode();
    }

    // DELETE: api/comments/1
    public async Task DeleteCommentAsync(
        int id, CancellationToken ct = default)
    {
        using var response = await http.DeleteAsync(
            $"api/comments/{id}", ct);

        response.EnsureSuccessStatusCode();
    }




    // HELPER METHOD

    // A 404 becomes null; any other failure throws.
    private async Task<T?> GetOrNullAsync<T>(
        string url, CancellationToken ct)
    {
        using var response = await http.GetAsync(url, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(ct);
    }
}
