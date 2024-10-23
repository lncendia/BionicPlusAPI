using AuthService.Models;

namespace AuthService.Dtos
{
    public class LoginResponse
    {
        public bool Success { get; init; }
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public Guid UserId { get; init; }
        public int RefreshTokenExpiryTime { get; init; }
        public AuthErrorCode? Code { get; init; }
    }
}
