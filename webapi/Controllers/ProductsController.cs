using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business;
using Domain;
namespace webapi.Controllers;
[ApiController, Authorize]
[Route("api/[controller]")]
public class ProductsController(IProductBusiness products) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll() => await products.GetAll();
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id) { var product = await products.Get(id); return product is null ? NotFound(new { message = "Producto no encontrado." }) : Ok(product); }
    [HttpPost]
    public async Task<ActionResult<Product>> Create(ProductRequest request) { var result = await products.Create(request); if (result.Error is not null) return Conflict(new { message = result.Error }); return CreatedAtAction(nameof(GetById), new { result.Product!.Id }, result.Product); }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductRequest request)
    {
        var error = await products.Update(id, request); if (error is null) return NoContent(); return error == "Producto no encontrado." ? NotFound(new { message = error }) : Conflict(new { message = error });
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => await products.Delete(id) ? NoContent() : NotFound(new { message = "Producto no encontrado." });
}
