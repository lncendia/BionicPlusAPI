using System.Security.Claims;

namespace AuthService.Services.Interfaces;

/// <summary>
/// Интерфейс для сервиса работы с JWT токенами.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Генерирует токен доступа на основе объекта ClaimsPrincipal.
    /// </summary>
    /// <param name="principal">Объект ClaimsPrincipal, содержащий информацию о пользователе.</param>
    /// <param name="issuer">Издатель токена.</param>
    /// <param name="tokenId"></param>
    /// <returns>Строка, представляющая токен доступа.</returns>
    string GenerateAccessToken(ClaimsPrincipal principal, string issuer, Guid tokenId);

    /// <summary>
    /// Получает объект ClaimsPrincipal из истекшего токена.
    /// </summary>
    /// <param name="token">Истекший токен.</param>
    /// <param name="refreshToken"></param>
    /// <param name="issuer">Издатель токена.</param>
    /// <returns>Объект ClaimsPrincipal, содержащий информацию о пользователе.</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string refreshToken, string issuer);

    /// <summary>
    /// Генерирует токен обновления и его время истечения.
    /// </summary>
    /// <param name="tokenId"></param>
    /// <returns>Кортеж, содержащий токен обновления и его время истечения.</returns>
    (string, DateTime) GenerateRefreshToken(Guid tokenId);
}
