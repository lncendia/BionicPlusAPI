using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Configuration;
using AuthService.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JsonWebKeySet = Microsoft.IdentityModel.Tokens.JsonWebKeySet;

namespace AuthService.Services.Implementations;

/// <summary>
/// Класс для управления внешними OIDC-провайдерами
/// </summary>
public class ExternalOidcManager : IExternalOidcManager
{
    /// <summary>
    /// Обработчик токенов JWT.
    /// </summary>
    private readonly JwtSecurityTokenHandler _handler = new();

    /// <summary>
    /// Аудитории токена.
    /// </summary>
    private readonly string[] _audiences;
    
    /// <summary>
    /// HTTP-клиент для выполнения запросов
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="settingsConfig">Конфигурация настроек</param>
    /// <param name="httpClient">HTTP-клиент для выполнения запросов</param>
    public ExternalOidcManager(IOptions<SettingsConfig> settingsConfig, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _audiences = settingsConfig.Value.AllowedExternalOidcAudiences.Split(',');
    }

    /// <inheritdoc/>
    /// <summary>
    /// Получает информацию о внешнем входе асинхронно
    /// </summary>
    public async Task<ExternalLoginInfo> GetLoginInfoAsync(string providerName, string identityToken)
    {
        // Читаем JWT-токен из строки идентификации
        var jwtToken = _handler.ReadJwtToken(identityToken);
        var keyId = jwtToken.Header.Kid;

        // Получаем документ открытия (discovery document) по URL JWKS
        var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = jwtToken.Issuer,
            Policy = new DiscoveryPolicy { ValidateEndpoints = false }
        });

        // Если KeySet равен null, выбрасываем исключение
        if (disco.KeySet == null) throw new SecurityTokenException("KeySet is null in the discovery document");

        // Создаем набор ключей JSON Web Key (JWK) из сырых данных KeySet
        var keySet = new JsonWebKeySet(disco.KeySet.RawData);

        // Находим ключ в наборе ключей по Kid
        var key = keySet.Keys.FirstOrDefault(k => k.Kid == keyId);

        // Если ключ не найден, выбрасываем исключение
        if (key == null) throw new SecurityTokenException($"Key with id '{keyId}' not found in the key set");

        // Настраиваем параметры валидации токена
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidAudiences = _audiences,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero
        };

        // Валидируем токен и получаем principal
        var principal = _handler.ValidateToken(identityToken, tokenValidationParameters, out _);

        // Находим значение Subject в principal
        var providerKey = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        // Если Subject не найден, выбрасываем исключение
        if (string.IsNullOrEmpty(providerKey)) throw new SecurityTokenException("Invalid token");

        // Возвращаем информацию о внешнем входе
        return new ExternalLoginInfo(principal, providerName, providerKey, providerName);
    }
}