namespace AuthService.Models;

public class RefreshTokenData
{
    public Guid TokenId { get; init; }
    public DateTime Expiration { get; init; }
}