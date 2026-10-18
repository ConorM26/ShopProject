
namespace ShopWeb.Models.Shop;

public record CreateOrderResult(
    OrderDto? Order,
    IReadOnlyList<string> Errors)
{
    public bool Succeeded => Order is not null;
}
