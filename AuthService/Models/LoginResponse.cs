namespace AuthService.Models;

public class LoginResponse : BaseAuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public int RefreshTokenExpiryTime { get; init; }
}