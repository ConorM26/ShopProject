using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWeb.Models.Shop;
using ShopWeb.Services;

namespace ShopWeb.Controllers;

[Authorize]
public class ItemsController(IShopApiClient shopApi) : Controller
{
    // GET /Items?search=mug
    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        IEnumerable<ItemDto> items = await shopApi.GetItemsAsync(ct);

        if (!string.IsNullOrWhiteSpace(search))
            items = items.Where(i =>
                i.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                ||
                (i.Description?.Contains(search,
                    StringComparison.OrdinalIgnoreCase) ?? false));

        var model = new ItemListViewModel
        {
            Items = items.OrderBy(i => i.Name).ToList(),
            Search = search
        };

        return View(model);
    }

    // GET /Items/Details/5
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await shopApi.GetItemAsync(id, ct);

        return item is null ? NotFound() : View(item);
    }
}
