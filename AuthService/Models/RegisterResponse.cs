namespace AuthService.Models;

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public AuthErrorCode? Code { get; set; }
}