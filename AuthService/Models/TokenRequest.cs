using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class TokenRequest
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}