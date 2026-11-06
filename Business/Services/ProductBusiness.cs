using Domain;
using Service;
namespace Business;
public class ProductBusiness(ProductRepository repository) : IProductBusiness
{
 public Task<List<Product>> GetAll() => repository.GetProducts(); public Task<Product?> Get(int id) => repository.GetProduct(id);
 public async Task<(Product? Product, string? Error)> Create(ProductRequest request) { if ((await repository.GetProducts()).Any(x => string.Equals(x.Code.Trim(), request.Code.Trim(), StringComparison.OrdinalIgnoreCase))) return (null, "El código de producto ya existe."); var product = Map(request); return (await repository.Add(product), null); }
 public async Task<string?> Update(int id, ProductRequest request) { var product = await repository.GetProduct(id); if (product is null) return "Producto no encontrado."; if ((await repository.GetProducts()).Any(x => string.Equals(x.Code.Trim(), request.Code.Trim(), StringComparison.OrdinalIgnoreCase) && x.Id != id)) return "El código de producto ya existe."; product.Code=request.Code.Trim(); product.Name=request.Name; product.Description=request.Description; product.IsActive=request.IsActive; product.Lots=request.Lots.Select(x => new ProductLot { LotNumber=x.LotNumber, Supplier=x.Supplier, Price=x.Price, Stock=x.Stock }).ToList(); await repository.Update(product); return null; }
 public async Task<bool> Delete(int id) { var product = await repository.GetProduct(id); if (product is null) return false; await repository.Delete(id); return true; }
 private static Product Map(ProductRequest request) => new() { Code=request.Code.Trim(), Name=request.Name, Description=request.Description, IsActive=request.IsActive, Lots=request.Lots.Select(x => new ProductLot { LotNumber=x.LotNumber, Supplier=x.Supplier, Price=x.Price, Stock=x.Stock }).ToList() };
}
