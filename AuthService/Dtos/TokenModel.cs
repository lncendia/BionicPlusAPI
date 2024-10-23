using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos;

public class TokenModel
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}