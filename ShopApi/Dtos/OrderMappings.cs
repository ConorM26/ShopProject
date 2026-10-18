
using ShopApi.Models;

namespace ShopApi.Dtos;

public static class OrderMappings
{
    // Run in memory after the query: SQLite cannot SUM decimal columns
    public static OrderDto ToDto(this Order order) => new(
        order.Id,
        order.CustomerName,
        order.OrderDate,
        order.Status,
        order.Lines.Sum(l => l.Quantity * l.UnitPrice),
        order.Lines
            .Select(l => new OrderLineDto(
                l.ItemId,
                l.Item?.Name ?? $"Item {l.ItemId}",
                l.Quantity,
                l.UnitPrice))
            .ToList());
}
