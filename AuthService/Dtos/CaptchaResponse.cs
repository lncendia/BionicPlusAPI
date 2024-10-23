using Newtonsoft.Json;

namespace AuthService.Dtos;

public class CaptchaResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("score")]
    public double Score { get; set; }
    
    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("challenge_ts")]
    public string ChallengeTs { get; set; }
    
    [JsonProperty("hostname")]
    public string Hostname { get; set; }

    [JsonProperty("error-codes")]
    public List<string> ErrorCodes { get; set; }
}