namespace AuthService.Infrastructure
{
    public class JwtConfig
    {
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudiences { get; set; } = string.Empty;
        public string IssuerSigningKey { get; set; } = string.Empty;
        public int RefreshTokenValidityInDays { get; set; } = 1;
    }
}
