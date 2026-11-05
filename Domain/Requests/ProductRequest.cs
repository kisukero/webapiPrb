using System.ComponentModel.DataAnnotations;
namespace Domain;
public class ProductRequest { [Required, MaxLength(50)] public string Code { get; set; } = string.Empty; [Required, MaxLength(150)] public string Name { get; set; } = string.Empty; [MaxLength(500)] public string? Description { get; set; } public bool IsActive { get; set; } = true; [MinLength(1)] public List<ProductLotRequest> Lots { get; set; } = []; }
public class ProductLotRequest { [Required, MaxLength(50)] public string LotNumber { get; set; } = string.Empty; [Required, MaxLength(150)] public string Supplier { get; set; } = string.Empty; [Range(0, 999999999)] public decimal Price { get; set; } [Range(0, int.MaxValue)] public int Stock { get; set; } }
