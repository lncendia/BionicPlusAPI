using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class RoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
