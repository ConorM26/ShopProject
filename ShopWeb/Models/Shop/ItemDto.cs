
namespace ShopWeb.Models.Shop;

public record ItemDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);
