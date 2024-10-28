using System.Text.Json.Serialization;
using AuthService.Infrastructure.Converters;
using DomainObjects.Pregnancy;

namespace AuthService.Models;

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