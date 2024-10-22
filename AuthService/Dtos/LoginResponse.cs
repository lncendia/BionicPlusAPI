using AuthService.Models;

namespace AuthService.Dtos
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int RefreshTokenExpiryTime { get; set; }
        public AuthErrorCode? Code { get; set; }
    }
}
