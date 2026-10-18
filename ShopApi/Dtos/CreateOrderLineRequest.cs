
using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos;

public record CreateOrderLineRequest
{
    [Range(1, int.MaxValue)]
    public int ItemId { get; init; }

    [Range(1, 1000)]
    public int Quantity { get; init; }
}
