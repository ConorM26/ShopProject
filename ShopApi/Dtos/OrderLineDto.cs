
namespace ShopApi.Dtos;

public record OrderLineDto(
    int ItemId,
    string ItemName,
    int Quantity,
    decimal UnitPrice);
