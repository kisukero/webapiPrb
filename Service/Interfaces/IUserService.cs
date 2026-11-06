using Domain;

namespace Service;

public interface IUserService
{
    Task<ApplicationUser?> GetUser(string userName);
}
