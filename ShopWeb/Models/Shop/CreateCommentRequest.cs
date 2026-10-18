
using System.ComponentModel.DataAnnotations;

namespace ShopWeb.Models.Shop;

public class CreateCommentRequest
{
    [Required]
    public int PostId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;
}
