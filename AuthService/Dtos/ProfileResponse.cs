using AuthService.Models;
using DomainObjects.Pregnancy;
using IdentityLibrary;
using IdentityLibrary.Models;
using System.Text.Json.Serialization;

namespace AuthService.Dtos
{
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
        public List<ApplicationRole>? Roles { get; set; }

        [JsonPropertyName("measureSystem")]
        public MeasureSystem? MeasureSystem { get; set; }

        [JsonPropertyName("height")]
        public double? Height { get; set; }
        
        [JsonPropertyName("subscription")]
        public ProfileSubscription ProfileSubscription { get; set; }
    }
}
