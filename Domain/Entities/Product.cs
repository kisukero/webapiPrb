using System.ComponentModel.DataAnnotations;
namespace Domain;
public class Product { public int Id { get; set; } [Required, MaxLength(50)] public string Code { get; set; } = string.Empty; [Required, MaxLength(150)] public string Name { get; set; } = string.Empty; [MaxLength(500)] public string? Description { get; set; } public bool IsActive { get; set; } = true; public List<ProductLot> Lots { get; set; } = []; }
public class ProductLot { public int Id { get; set; } [Required, MaxLength(50)] public string LotNumber { get; set; } = string.Empty; [Required, MaxLength(150)] public string Supplier { get; set; } = string.Empty; public decimal Price { get; set; } public int Stock { get; set; } public int ProductId { get; set; } public Product? Product { get; set; } }
