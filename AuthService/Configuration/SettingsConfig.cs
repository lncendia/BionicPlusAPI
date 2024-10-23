using System.Text.Json.Serialization;

namespace AuthService.Configuration;

public class SettingsConfig
{
    [JsonPropertyName("captchaKey")]
    public string CaptchaKey { get; set; } = string.Empty;

    [JsonPropertyName("googleClientId")]
    public string GoogleClientId { get; set; } = string.Empty;
}