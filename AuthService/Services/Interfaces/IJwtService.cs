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
    /// <returns>Строка, представляющая токен доступа.</returns>
    string GenerateAccessToken(ClaimsPrincipal principal);

    /// <summary>
    /// Получает объект ClaimsPrincipal из истекшего токена.
    /// </summary>
    /// <param name="token">Истекший токен.</param>
    /// <returns>Объект ClaimsPrincipal, содержащий информацию о пользователе.</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// Генерирует токен обновления и его время истечения.
    /// </summary>
    /// <returns>Кортеж, содержащий токен обновления и его время истечения.</returns>
    (string, TimeSpan) GenerateRefreshToken();
}
