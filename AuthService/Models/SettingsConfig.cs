using System.Text.Json.Serialization;

namespace AuthService.Models
{
    public class SettingsConfig
    {
        [JsonPropertyName("captchaKey")]
        public string CaptchaKey { get; set; } = string.Empty;

        [JsonPropertyName("googleClientId")]
        public string GoogleClientId { get; set; } = string.Empty;
    }
}
