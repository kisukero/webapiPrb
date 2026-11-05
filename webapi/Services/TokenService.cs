using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Domain;
namespace webapi.Services;
public class TokenService(IConfiguration configuration)
{
    public string Create(ApplicationUser user)
    {
        var jwt = configuration.GetSection("Jwt"); var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.UserName) };
        var key = new SymmetricSecurityKey(SHA256.HashData(Encoding.UTF8.GetBytes(jwt["Key"]!))); var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); var expires = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpirationMinutes"));
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, expires: expires, signingCredentials: credentials));
    }
}
