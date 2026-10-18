using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApi.Models;

public class OrderLine
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    // Ignored in JSON to stop an Order -> Lines -> Order loop
    [JsonIgnore]
    public Order? Order { get; set; }

    public int ItemId { get; set; }

    public Item? Item { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}