
using System.ComponentModel.DataAnnotations;

namespace ShopApi.Models;

public class Order
{
    public int Id { get; set; }

    // Identity user id from the JWT "sub" claim
    [Required, MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public List<OrderLine> Lines { get; set; } = [];
}
