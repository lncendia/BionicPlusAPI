namespace AuthService.Models;

public class RefreshResponse
{
    public bool Success { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public int RefreshTokenExpiryTime { get; init; }
}