using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

public class ClientSettingsConfig
{
    private readonly SettingsConfig _baseConfig;
    
    public ClientSettingsConfig(SettingsConfig config)
    {
        _baseConfig = config;
    }
    
    [JsonPropertyName("captchaKey")]
    public string CaptchaKey => _baseConfig.CaptchaKey;
    
    [JsonPropertyName("googleClientIdWeb")]
    public string GoogleClientIdWeb => _baseConfig.GoogleClientIdWeb;
    
    [JsonPropertyName("googleClientIdAndroid")]
    public string GoogleClientIdAndroid => _baseConfig.GoogleClientIdAndroid;
}