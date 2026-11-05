using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business;
using Domain;
using webapi.Services;
namespace webapi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthBusiness auth, TokenService tokens) : ControllerBase
{
    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await auth.Validate(request);
        if (user is null) return Unauthorized(new { message = "Usuario o contraseña inválidos." });
        return Ok(new { token = tokens.Create(user), user = new { user.Id, user.UserName, user.FullName } });
    }
    [Authorize, HttpGet("session")]
    public IActionResult Session() => Ok(new { userId = User.FindFirstValue(ClaimTypes.NameIdentifier), userName = User.Identity?.Name });
}
