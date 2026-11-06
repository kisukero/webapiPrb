using Domain;

namespace Service;

public interface IProductService
{
    Task<List<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
    Task<Product> Add(Product product);
    Task Update(Product product);
    Task Delete(int id);
}
