using System.ComponentModel.DataAnnotations;
namespace Domain;
public class ApplicationUser { public int Id { get; set; } [MaxLength(50)] public string UserName { get; set; } = string.Empty; [MaxLength(150)] public string FullName { get; set; } = string.Empty; [MaxLength(500)] public string PasswordHash { get; set; } = string.Empty; public bool IsActive { get; set; } }
