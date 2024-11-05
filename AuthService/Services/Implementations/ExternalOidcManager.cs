using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JsonWebKeySet = Microsoft.IdentityModel.Tokens.JsonWebKeySet;

namespace AuthService.Services.Implementations;

/// <summary>
/// Класс для управления внешними OIDC-провайдерами
/// </summary>
public class ExternalOidcManager : IExternalProcessor
{
    /// <summary>
    /// Обработчик токенов JWT.
    /// </summary>
    private readonly JwtSecurityTokenHandler _handler = new();

    /// <summary>
    /// Array of audiences.
    /// </summary>
    private readonly string[] _audiences;

    /// <summary>
    /// Issuer string.
    /// </summary>
    private readonly string _issuer;

    /// <summary>
    /// Provider name.
    /// </summary>
    private readonly string _providerName;

    /// <summary>
    /// This class manages external OpenID Connect (OIDC) operations.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the ExternalOidcManager class.
    /// </summary>
    /// <param name="providerName">The name of the OIDC provider.</param>
    /// <param name="audiences">The audiences for the OIDC tokens.</param>
    /// <param name="issuer">The issuer of the OIDC tokens.</param>
    /// <param name="httpClient">The HTTP client used for making requests.</param>
    public ExternalOidcManager(string providerName, string[] audiences, string issuer, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _audiences = audiences;
        _issuer = issuer;
        _providerName = providerName;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Получает информацию о внешнем входе асинхронно
    /// </summary>
    public async Task<ExternalLoginInfo> GetLoginInfoAsync(string identityToken)
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
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero
        };

        // Валидируем токен и получаем principal
        var principal = _handler.ValidateToken(identityToken, tokenValidationParameters, out _);

        // Находим идентификатор пользователя в principal
        var providerKey = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        // Если идентификатор не найден, выбрасываем исключение
        if (string.IsNullOrEmpty(providerKey)) throw new SecurityTokenException("Invalid token");

        // Возвращаем информацию о внешнем входе
        return new ExternalLoginInfo(principal, _providerName, providerKey, _providerName);
    }
}