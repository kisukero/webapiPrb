using Domain;
namespace Business;
public interface IProductBusiness { Task<List<Product>> GetAll(); Task<Product?> Get(int id); Task<(Product? Product, string? Error)> Create(ProductRequest request); Task<string?> Update(int id, ProductRequest request); Task<bool> Delete(int id); }
