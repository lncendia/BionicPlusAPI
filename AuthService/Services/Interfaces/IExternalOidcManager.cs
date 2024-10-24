using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.Interfaces;

/// <summary>
/// Интерфейс для управления внешними OIDC-провайдерами
/// </summary>
public interface IExternalOidcManager
{
    /// <summary>
    /// Получает информацию о внешнем входе асинхронно
    /// </summary>
    /// <param name="providerName">Имя провайдера входа</param>
    /// <param name="identityToken">Токен идентификации</param>
    /// <returns>Информация о внешнем входе</returns>
    Task<ExternalLoginInfo> GetLoginInfoAsync(string providerName, string identityToken);
}