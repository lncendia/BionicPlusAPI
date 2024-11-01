namespace AuthService.Models;

public class BaseAuthResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public AuthErrorCode? Code { get; set; }
}