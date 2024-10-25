using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Configuration;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services.Implementations;

public class JwtService : IJwtService
{
    /// <summary>
    /// Обработчик токенов JWT.
    /// </summary>
    private readonly JwtSecurityTokenHandler _handler = new();

    /// <summary>
    /// Учетные данные для подписи токена.
    /// </summary>
    private readonly SigningCredentials _credentials;

    /// <summary>
    /// Аудитории токена.
    /// </summary>
    private readonly string[] _audiences;
    
    /// <summary>
    /// Издатель токена.
    /// </summary>
    private readonly string _issuer;

    /// <summary>
    /// Время жизни токена доступа.
    /// </summary>
    private readonly TimeSpan _accessTokenLifetime;

    /// <summary>
    /// Время жизни токена обновления.
    /// </summary>
    private readonly TimeSpan _refreshTokenLifetime;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="jwtConfig">Конфигурация JWT</param>
    public JwtService(IOptions<JwtConfig> jwtConfig)
    {
        // Устанавливаем аудиторию токена
        _audiences = jwtConfig.Value.IssuedAudiences.Split(',');

        // Устанавливаем издателя токена
        _issuer = jwtConfig.Value.ValidIssuer;

        // Устанавливаем время жизни токена обновления
        _refreshTokenLifetime = TimeSpan.FromDays(jwtConfig.Value.RefreshTokenValidityInDays);

        // Устанавливаем время жизни токена доступа
        _accessTokenLifetime = TimeSpan.FromHours(3);

        // Создаем симметричный ключ безопасности на основе секретного ключа
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Value.IssuerSigningKey));

        // Создаем учетные данные для подписи токена
        _credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Генерирует токен доступа на основе объекта ClaimsPrincipal.
    /// </summary>
    public string GenerateAccessToken(ClaimsPrincipal principal)
    {
        var claims = principal.Claims.ToList();
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.AddRange(_audiences.Select(audience => new Claim(JwtRegisteredClaimNames.Aud, audience)));

        // Создаем токен JWT
        var accessToken = new JwtSecurityToken(
            issuer: _issuer,
            claims: claims,
            expires: DateTime.Now.Add(_accessTokenLifetime),
            signingCredentials: _credentials
        );

        // Возвращаем строку, представляющую сгенерированный токен доступа
        return _handler.WriteToken(accessToken);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Получает объект ClaimsPrincipal из истекшего токена.
    /// </summary>
    public (string, TimeSpan) GenerateRefreshToken()
    {
        // Создание массива байтов для генерации случайного числа
        var randomNumber = new byte[64];

        // Использование генератора случайных чисел для заполнения массива
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        // Преобразование массива байтов в строку Base64
        return (Convert.ToBase64String(randomNumber), _refreshTokenLifetime);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Генерирует токен обновления и его время истечения.
    /// </summary>
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        // Настройка параметров валидации токена
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = _audiences,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _credentials.Key,
            ValidateLifetime = false // Не проверять срок действия токена
        };

        try
        {
            // Валидация токена и получение объекта ClaimsPrincipal
            var principal = _handler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch (ArgumentException)
        {
            // Если токен недействителен, выбрасываем исключение
            throw new SecurityTokenException("Invalid token");
        }
    }
}