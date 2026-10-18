
using Microsoft.EntityFrameworkCore;
using ShopApi.Models;

namespace ShopApi.Data;

public class ShopContext(DbContextOptions<ShopContext> options)
    : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.UserId);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Lines)
            .WithOne(l => l.Order)
            .HasForeignKey(l => l.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderLine>()
            .HasOne(l => l.Item)
            .WithMany()
            .HasForeignKey(l => l.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Item>().HasData(
            new Item
            {
                Id = 1,
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 79.99m,
                StockQuantity = 25
            },
            new Item
            {
                Id = 2,
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 24.50m,
                StockQuantity = 60
            },
            new Item
            {
                Id = 3,
                Name = "Monitor",
                Description = "27-inch 4K monitor",
                Price = 329.00m,
                StockQuantity = 10
            });
    }
}
