
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(ShopContext context) : ControllerBase
{
    // The signed-in user's id, from the token's "sub" claim
    private string CurrentUserId =>
        User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? throw new InvalidOperationException(
            "The access token has no 'sub' claim.");

    // GET: api/Orders (only the caller's orders)
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders(
        CancellationToken ct)
    {
        var userId = CurrentUserId;

        var orders = await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.Lines)
            .ThenInclude(l => l.Item)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(ct);

        return Ok(orders.Select(o => o.ToDto()).ToList());
    }

    // GET: api/Orders/5 (404 if it doesn't exist or isn't yours)
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(
        int id, CancellationToken ct)
    {
        var userId = CurrentUserId;

        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .ThenInclude(l => l.Item)
            .FirstOrDefaultAsync(
                o => o.Id == id && o.UserId == userId, ct);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order.ToDto());
    }

    // POST: api/Orders
    [HttpPost]
    public async Task<ActionResult<OrderDto>> PostOrder(
        CreateOrderRequest request, CancellationToken ct)
    {
        // Merge repeated item ids into one line each
        var requested = request.Lines
            .GroupBy(l => l.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                Quantity = g.Sum(l => l.Quantity)
            })
            .ToList();

        var itemIds = requested.Select(r => r.ItemId).ToList();

        var items = await context.Items
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, ct);

        foreach (var line in requested)
        {
            if (!items.TryGetValue(line.ItemId, out var item))
            {
                ModelState.AddModelError(
                    nameof(request.Lines),
                    $"Item {line.ItemId} does not exist.");
            }
            else if (item.StockQuantity < line.Quantity)
            {
                ModelState.AddModelError(
                    nameof(request.Lines),
                    $"Only {item.StockQuantity} of '{item.Name}' left in stock.");
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var order = new Order
        {
            UserId = CurrentUserId,
            CustomerName = User.Identity?.Name ?? "Unknown",
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        foreach (var line in requested)
        {
            var item = items[line.ItemId];

            item.StockQuantity -= line.Quantity;

            order.Lines.Add(new OrderLine
            {
                ItemId = item.Id,
                Item = item,
                Quantity = line.Quantity,
                UnitPrice = item.Price // Price comes from the database
            });
        }

        context.Orders.Add(order);

        // Save order, order lines and stock changes
        await context.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = order.Id },
            order.ToDto());
    }
}
