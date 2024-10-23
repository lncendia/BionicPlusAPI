namespace AuthService.Configuration;

public class JwtConfig
{
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudiences { get; set; } = string.Empty;
    
    
    public string IssuedAudiences { get; set; } = string.Empty;
    public string IssuerSigningKey { get; set; } = string.Empty;
    public int RefreshTokenValidityInDays { get; set; } = 1;
}