using AuthService.Infrastructure.Converters;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.UserProfile;
using IdentityLibrary;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthService.Dtos
{
    public class ProfileRequest
    {
        [JsonPropertyName("statuses")]
        [JsonConverter(typeof(CategoryListConverter))]
        public List<Category>? Statuses { get; set; }

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("height")]
        public double? Height { get; set; }
    }
}
