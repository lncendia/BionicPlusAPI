using System.Text.Json.Serialization;

namespace AuthService.Configuration;

public class SettingsConfig
{
    [JsonPropertyName("captchaKey")]
    public string CaptchaKey { get; set; } = string.Empty;
    
    [JsonPropertyName("captchaSecret")]
    public string CaptchaSecret { get; set; } = string.Empty;

    [JsonPropertyName("googleClientIdWeb")]
    public string GoogleClientIdWeb { get; set; } = string.Empty;
    
    [JsonPropertyName("googleClientIdAndroid")]
    public string GoogleClientIdAndroid { get; set; } = string.Empty;
    
    [JsonPropertyName("externalGoogleAudiences")]
    public string ExternalGoogleAudiences { get; set; } = string.Empty;
}