using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

/// <summary>
/// Конфигурация настроек клиента.
/// </summary>
public class ClientSettingsConfig
{
    /// <summary>
    /// Базовая конфигурация настроек.
    /// </summary>
    private readonly SettingsConfig _baseConfig;

    /// <summary>
    /// Конструктор класса ClientSettingsConfig.
    /// </summary>
    /// <param name="config">Базовая конфигурация настроек.</param>
    public ClientSettingsConfig(SettingsConfig config)
    {
        _baseConfig = config;
    }

    /// <summary>
    /// Ключ для капчи.
    /// </summary>
    [JsonPropertyName("captchaKey")]
    public string CaptchaKey => _baseConfig.CaptchaKey;
    
    /// <summary>
    /// Идентификатор клиента Google для Веб.
    /// </summary>
    [JsonPropertyName("googleClientIdWeb")]
    public string GoogleClientIdWeb => _baseConfig.GoogleClientIdWeb;
    
    /// <summary>
    /// Идентификатор клиента Google для Android.
    /// </summary>
    [JsonPropertyName("googleClientIdAndroid")]
    public string GoogleClientIdAndroid => _baseConfig.GoogleClientIdAndroid;
}