using Domain;

namespace Service;

public class ProductRepository(IProductService storedProcedures)
{
    public Task<List<Product>> GetProducts() => storedProcedures.GetProducts();
    public Task<Product?> GetProduct(int id) => storedProcedures.GetProduct(id);
    public Task<Product> Add(Product product) => storedProcedures.Add(product);
    public Task Update(Product product) => storedProcedures.Update(product);
    public Task Delete(int id) => storedProcedures.Delete(id);
}
