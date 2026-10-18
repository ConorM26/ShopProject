
namespace ShopApi.Dtos;

public record OrderDto(
    int Id,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    decimal Total,
    IReadOnlyList<OrderLineDto> Items);
