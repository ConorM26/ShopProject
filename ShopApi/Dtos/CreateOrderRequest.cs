
using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos;

public record CreateOrderRequest
{
    [Required, MinLength(1), MaxLength(50)]
    public List<CreateOrderLineRequest> Lines { get; init; } = [];
}
