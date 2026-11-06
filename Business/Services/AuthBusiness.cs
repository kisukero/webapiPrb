using Domain;
using Microsoft.AspNetCore.Identity;
using Service;
namespace Business;
public class AuthBusiness(UserRepository repository) : IAuthBusiness
{
 public async Task<ApplicationUser?> Validate(LoginRequest request) { var user = await repository.GetUser(request.UserName); return user is not null && new PasswordHasher<ApplicationUser>().VerifyHashedPassword(user, user.PasswordHash, request.Password) != PasswordVerificationResult.Failed ? user : null; }
}
