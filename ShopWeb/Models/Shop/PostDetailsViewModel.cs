
namespace ShopWeb.Models.Shop;

public class PostDetailsViewModel
{
    public PostDto Post { get; set; } = new();

    public IReadOnlyList<CommentDto> Comments { get; set; }
        = new List<CommentDto>();

    public CreateCommentRequest NewComment { get; set; } = new();
}
