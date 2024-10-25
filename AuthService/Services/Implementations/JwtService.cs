using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Configuration;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AuthService.Services.Implementations;

/// <summary>
/// Сервис для работы с токенами
/// </summary>
public class JwtService : IJwtService, IDisposable
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
    /// Объект для шифрования Aes.
    /// </summary>
    private readonly SymmetricAlgorithm _algorithm;

    /// <summary>
    /// Ключ для шифрования токена обновления.
    /// </summary>
    private readonly byte[] _refreshTokenKey;

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

        // Устанавливаем ключ для токена обновления
        _refreshTokenKey = Encoding.UTF8.GetBytes(jwtConfig.Value.IssuerSigningKey);

        // Создаем симметричный ключ безопасности на основе секретного ключа
        var securityKey = new SymmetricSecurityKey(_refreshTokenKey);

        // Создаем учетные данные для подписи токена
        _credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Создаем объект для шифрования Aes
        _algorithm = Aes.Create();
    }

    /// <inheritdoc/>
    /// <summary>
    /// Генерирует токен доступа на основе объекта ClaimsPrincipal.
    /// </summary>
    public string GenerateAccessToken(ClaimsPrincipal principal, Guid tokenId)
    {
        // Преобразуем ClaimsPrincipal в список Claims
        var claims = principal.Claims.ToList();

        // Добавляем утверждение JTI (JWT ID) с идентификатором токена
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString()));

        // Добавляем утверждения для каждой аудитории из списка аудиторий
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
    /// <param name="tokenId">Идентификатор токена.</param>
    public (string, DateTime) GenerateRefreshToken(Guid tokenId)
    {
        // Создаем данные для токена обновления
        var refreshTokenPayload = new RefreshTokenData
        {
            TokenId = tokenId,
            Expiration = DateTime.Now.Add(_refreshTokenLifetime)
        };

        // Сериализуем данные токена обновления в JSON
        var refreshTokenJson = JsonConvert.SerializeObject(refreshTokenPayload);

        // Шифруем JSON данные токена обновления
        var token = Encrypt(refreshTokenJson);

        // Возвращаем зашифрованный токен и время его истечения
        return (token, refreshTokenPayload.Expiration);
    }

    /// <inheritdoc/>
    /// <summary>
    /// Генерирует токен обновления и его время истечения.
    /// </summary>
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string refreshToken)
    {
        // Расшифровываем токен обновления
        var refreshTokenJson = Decrypt(token);

        // Десериализуем JSON данные токена обновления
        var tokenPayload = JsonConvert.DeserializeObject<RefreshTokenData>(refreshTokenJson)!;

        // Проверяем, что токен обновления еще не истек
        if (tokenPayload.Expiration < DateTime.Now) throw new SecurityTokenException("Invalid token");

        // Настраиваем параметры валидации токена
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = _audiences,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _credentials.Key,
            ValidateLifetime = false
        };

        try
        {
            // Валидация токена и получение объекта ClaimsPrincipal
            var principal = _handler.ValidateToken(token, tokenValidationParameters, out var parsedToken);

            // Проверяем, что идентификатор токена можно преобразовать в Guid
            if (Guid.TryParse(parsedToken.Id, out var parsedTokenId)) throw new SecurityTokenException("Invalid token");

            // Проверяем, что идентификатор токена совпадает с идентификатором токена обновления
            if (parsedTokenId != tokenPayload.TokenId) throw new SecurityTokenException("Invalid token");

            // Возвращаем объект ClaimsPrincipal
            return principal;
        }
        catch (ArgumentException)
        {
            // Выбрасываем исключение, если токен невалиден
            throw new SecurityTokenException("Invalid token");
        }
    }

    /// <summary>
    /// Шифрует данные.
    /// </summary>
    /// <param name="data">Данные для шифрования.</param>
    /// <returns>Зашифрованные данные.</returns>
    private string Encrypt(string data)
    {
        // Создаем объект шифрования
        using var encryptor = _algorithm.CreateEncryptor(_refreshTokenKey, _algorithm.IV);

        // Преобразуем данные в байты
        var dataBytes = Encoding.UTF8.GetBytes(data);

        // Шифруем данные
        var encryptedData = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

        // Преобразуем зашифрованные данные в строку Base64
        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// Расшифровывает данные.
    /// </summary>
    /// <param name="encryptedData">Зашифрованные данные.</param>
    /// <returns>Расшифрованные данные.</returns>
    private string Decrypt(string encryptedData)
    {
        // Создаем объект расшифрования
        using var decryptor = _algorithm.CreateDecryptor(_refreshTokenKey, _algorithm.IV);

        // Преобразуем зашифрованные данные из строки Base64 в байты
        var encryptedDataBytes = Convert.FromBase64String(encryptedData);

        // Расшифровываем данные
        var decryptedData = decryptor.TransformFinalBlock(encryptedDataBytes, 0, encryptedDataBytes.Length);

        // Преобразуем расшифрованные данные в строку
        return Encoding.UTF8.GetString(decryptedData);
    }

    /// <summary>
    /// Освобождает ресурсы.
    /// </summary>
    public void Dispose()
    {
        // Подавляем финализацию для этого объекта
        GC.SuppressFinalize(this);

        // Освобождаем ресурсы алгоритма шифрования
        _algorithm.Dispose();
    }
}