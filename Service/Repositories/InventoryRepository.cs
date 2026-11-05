using Domain;
using Microsoft.EntityFrameworkCore;
namespace Service;
public class InventoryRepository(InventoryDbContext db) : IInventoryRepository
{
 public Task<ApplicationUser?> GetUser(string userName) => db.Users.SingleOrDefaultAsync(x => x.UserName == userName && x.IsActive);
 public Task<bool> ProductCodeExists(string code, int? excludingId = null) => db.Products.AnyAsync(x => x.Code == code && (!excludingId.HasValue || x.Id != excludingId));
 public Task<List<Product>> GetProducts() => db.Products.AsNoTracking().Include(x => x.Lots).OrderBy(x => x.Name).ToListAsync();
 public Task<Product?> GetProduct(int id, bool track = false) => (track ? db.Products : db.Products.AsNoTracking()).Include(x => x.Lots).SingleOrDefaultAsync(x => x.Id == id);
 public async Task<Product> Add(Product product) { db.Products.Add(product); await db.SaveChangesAsync(); return product; }
 public Task Save() => db.SaveChangesAsync(); public void Delete(Product product) => db.Products.Remove(product);
}
