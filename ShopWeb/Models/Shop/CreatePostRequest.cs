
using System.ComponentModel.DataAnnotations;

namespace ShopWeb.Models.Shop;

public class CreatePostRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}
