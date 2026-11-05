using Domain;
namespace Business;
public interface IAuthBusiness { Task<ApplicationUser?> Validate(LoginRequest request); }
