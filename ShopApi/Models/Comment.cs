
using System.ComponentModel.DataAnnotations;

namespace ShopApi.Models;

public class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
