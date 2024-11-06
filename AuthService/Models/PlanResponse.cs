using System.Text.Json.Serialization;

namespace AuthService.Models;

public class PlanResponse
{
    [JsonPropertyName("planName")]
    public string? PlanName { get; set; }

    [JsonPropertyName("isFreePlan")]
    public bool IsFreePlan { get; set; }

    [JsonPropertyName("surveyLimit")]
    public int SurveyLimit { get; set; }

    [JsonPropertyName("rollbackLimit")]
    public int RollbackLimit { get; set; }
}