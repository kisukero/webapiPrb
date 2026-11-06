using Domain;

namespace Service;

public class UserRepository(IUserService storedProcedures)
{
    public Task<ApplicationUser?> GetUser(string userName) => storedProcedures.GetUser(userName);
}
