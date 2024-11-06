using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class RoleRequest
{
    [Required]
    public string Role { get; set; } = string.Empty;
}