using DomainObjects.Pregnancy;
using IdentityLibrary;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Dtos;

public class ProfileResponse
{
    [JsonPropertyName("isEmailConfirmed")]
    public bool IsEmailConfirmed { get; set; }

    [JsonPropertyName("temperatureUnits")]
    public TemperatureUnits TemperatureUnits { get; set; }

    [JsonPropertyName("userId")]
    public Guid? UserId { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("statuses")]
    public List<Category>? Statuses { get; set; }

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("planId")]
    public string? PlanId { get; set; }

    [JsonPropertyName("roles")]
    public IList<string>? Roles { get; set; }

    [JsonPropertyName("measureSystem")]
    public MeasureSystem? MeasureSystem { get; set; }

    [JsonPropertyName("height")]
    public double? Height { get; set; }

    [JsonPropertyName("logins")]
    public IList<UserLoginInfo>? Logins { get; set; }
}