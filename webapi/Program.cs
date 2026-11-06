using Business;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Service;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using webapi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, config) => config.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services));
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<InventoryDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("PrbBgSql")));
builder.Services.AddScoped<IProductService, ProductStoredProcedureService>();
builder.Services.AddScoped<IUserService, UserStoredProcedureService>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IAuthBusiness, AuthBusiness>();
builder.Services.AddScoped<IProductBusiness, ProductBusiness>();
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"] ?? throw new InvalidOperationException("Falta Jwt:Key.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
    ValidIssuer = jwt["Issuer"], ValidAudience = jwt["Audience"], IssuerSigningKey = new SymmetricSecurityKey(SHA256.HashData(Encoding.UTF8.GetBytes(jwtKey)))
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();
builder.Services.AddCors(o => o.AddPolicy("Angular", p => p.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? []).AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(e => e.Run(async c => { c.Response.StatusCode = 500; await c.Response.WriteAsJsonAsync(new { message = "Ocurrió un error al procesar la solicitud." }); }));
app.UseSwagger();
app.UseSwaggerUI();
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    if (!await db.Users.AnyAsync())
    {
        var user = new ApplicationUser { UserName = "admin", FullName = "Administrador", IsActive = true };
        user.PasswordHash = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>().HashPassword(user, "Admin123*");
        db.Users.Add(user); await db.SaveChangesAsync();
    }
}
app.Run();
