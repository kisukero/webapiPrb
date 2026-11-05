using Domain;
namespace Service;
public interface IInventoryRepository { Task<ApplicationUser?> GetUser(string userName); Task<bool> ProductCodeExists(string code, int? excludingId = null); Task<List<Product>> GetProducts(); Task<Product?> GetProduct(int id, bool track = false); Task<Product> Add(Product product); Task Save(); void Delete(Product product); }
