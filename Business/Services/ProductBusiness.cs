using Domain;
using Service;
namespace Business;
public class ProductBusiness(IInventoryRepository repository) : IProductBusiness
{
 public Task<List<Product>> GetAll() => repository.GetProducts(); public Task<Product?> Get(int id) => repository.GetProduct(id);
 public async Task<(Product? Product, string? Error)> Create(ProductRequest request) { if (await repository.ProductCodeExists(request.Code)) return (null, "El código de producto ya existe."); var product = Map(request); return (await repository.Add(product), null); }
 public async Task<string?> Update(int id, ProductRequest request) { var product = await repository.GetProduct(id, true); if (product is null) return "Producto no encontrado."; if (await repository.ProductCodeExists(request.Code, id)) return "El código de producto ya existe."; product.Code=request.Code; product.Name=request.Name; product.Description=request.Description; product.IsActive=request.IsActive; product.Lots.Clear(); product.Lots.AddRange(request.Lots.Select(x => new ProductLot { LotNumber=x.LotNumber, Supplier=x.Supplier, Price=x.Price, Stock=x.Stock })); await repository.Save(); return null; }
 public async Task<bool> Delete(int id) { var product = await repository.GetProduct(id, true); if (product is null) return false; repository.Delete(product); await repository.Save(); return true; }
 private static Product Map(ProductRequest request) => new() { Code=request.Code, Name=request.Name, Description=request.Description, IsActive=request.IsActive, Lots=request.Lots.Select(x => new ProductLot { LotNumber=x.LotNumber, Supplier=x.Supplier, Price=x.Price, Stock=x.Stock }).ToList() };
}
