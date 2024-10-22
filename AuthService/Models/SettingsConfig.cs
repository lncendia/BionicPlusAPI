namespace AuthService.Models
{
    public class SettingsConfig
    {
        public string CaptchaKey { get; set; } = string.Empty;
        
        public string CaptchaSecret { get; set; } = string.Empty;
        
        public string GoogleClientId { get; set; } = string.Empty;
    }
}
