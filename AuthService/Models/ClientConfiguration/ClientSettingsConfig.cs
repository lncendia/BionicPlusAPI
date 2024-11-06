using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

/// <summary>
/// Configuration for client settings.
/// </summary>
public class ClientSettingsConfig
{
    /// <summary>
    /// Base configuration for settings.
    /// </summary>
    private readonly SettingsConfig _baseConfig;

    /// <summary>
    /// Constructor for the ClientSettingsConfig class.
    /// </summary>
    /// <param name="config">Base configuration for settings.</param>
    public ClientSettingsConfig(SettingsConfig config)
    {
        _baseConfig = config;
    }

    /// <summary>
    /// Key for captcha.
    /// </summary>
    [JsonPropertyName("captchaKey")]
    public string CaptchaKey => _baseConfig.CaptchaKey;

    /// <summary>
    /// Google client ID for Web.
    /// </summary>
    [JsonPropertyName("googleClientIdWeb")]
    public string GoogleClientIdWeb => _baseConfig.GoogleClientIdWeb;

    /// <summary>
    /// Google client ID for Android.
    /// </summary>
    [JsonPropertyName("googleClientIdAndroid")]
    public string GoogleClientIdAndroid => _baseConfig.GoogleClientIdAndroid;
}
