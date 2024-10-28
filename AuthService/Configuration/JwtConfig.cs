namespace AuthService.Configuration;

/// <summary>
/// Конфигурация JWT.
/// </summary>
public class JwtConfig
{
    /// <summary>
    /// Допустимый издатель токена.
    /// </summary>
    public string ValidIssuer { get; set; } = string.Empty;

    /// <summary>
    /// Допустимые аудитории токена.
    /// </summary>
    public string ValidAudiences { get; set; } = string.Empty;

    /// <summary>
    /// Аудитории, для которых выпущен токен.
    /// </summary>
    public string IssuedAudiences { get; set; } = string.Empty;

    /// <summary>
    /// Ключ подписи издателя.
    /// </summary>
    public string IssuerSigningKey { get; set; } = string.Empty;

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Вектор инициализации для обновления токена.
    /// </summary>
    public string RefreshTokenIV { get; set; } = string.Empty;

    /// <summary>
    /// Срок действия обновления токена в днях.
    /// </summary>
    public int RefreshTokenValidityInDays { get; set; } = 1;
}
