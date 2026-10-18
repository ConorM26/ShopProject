
using ShopWeb.Models.Shop;

namespace ShopWeb.Services;

public interface IShopApiClient
{
    // Items
    Task<IReadOnlyList<ItemDto>> GetItemsAsync(
        CancellationToken ct = default);

    Task<ItemDto?> GetItemAsync(
        int id, CancellationToken ct = default);

    // Orders
    Task<IReadOnlyList<OrderDto>> GetOrdersAsync(
        CancellationToken ct = default);

    Task<OrderDto?> GetOrderAsync(
        int id, CancellationToken ct = default);

    Task<CreateOrderResult> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken ct = default);

    // Posts
    Task<IReadOnlyList<PostDto>> GetPostsAsync(
        CancellationToken ct = default);

    Task<PostDto?> GetPostAsync(
        int id, CancellationToken ct = default);

    Task<PostDto> CreatePostAsync(
        CreatePostRequest request,
        CancellationToken ct = default);

    Task UpdatePostAsync(
        int id, CreatePostRequest request,
        CancellationToken ct = default);

    Task DeletePostAsync(
        int id, CancellationToken ct = default);


    // Get comments belonging to a post
    Task<IReadOnlyList<CommentDto>> GetCommentsAsync(
        int postId,
        CancellationToken ct = default);

    // Create a new comment
    Task<CommentDto> CreateCommentAsync(
        CreateCommentRequest request,
        CancellationToken ct = default);


    // Update an existing comment
    Task UpdateCommentAsync(
        int id,
        string content,
        CancellationToken ct = default);

    // Delete a comment
    Task DeleteCommentAsync(
        int id,
        CancellationToken ct = default);


}
