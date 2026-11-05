using Domain;
using Microsoft.EntityFrameworkCore;
namespace Service;
public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
 public DbSet<ApplicationUser> Users => Set<ApplicationUser>(); public DbSet<Product> Products => Set<Product>(); public DbSet<ProductLot> ProductLots => Set<ProductLot>();
 protected override void OnModelCreating(ModelBuilder b) { b.Entity<ApplicationUser>(e => { e.ToTable("InventoryUsers"); e.HasIndex(x => x.UserName).IsUnique(); }); b.Entity<Product>(e => { e.ToTable("InventoryProducts"); e.HasIndex(x => x.Code).IsUnique(); }); b.Entity<ProductLot>(e => { e.ToTable("InventoryProductLots"); e.Property(x => x.Price).HasColumnType("decimal(18,2)"); e.HasIndex(x => new { x.ProductId, x.LotNumber, x.Supplier }).IsUnique(); e.HasOne(x => x.Product).WithMany(x => x.Lots).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade); }); }
}
