using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DomainObjects.Pregnancy
{
    public class Card
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        [Required]
        [StringLength(maximumLength: 200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("solutionSteps")]
        public string? SolutionSteps { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("references")]
        public string? References { get; set; }

        [JsonPropertyName("answers")]
        public List<Answer>? Answers { get; set; }

        [JsonPropertyName("userInputType")]
        public UserInputType? UserInputType { get; set; }

        [JsonPropertyName("conditions")]
        public Condition? Condition { get; set; }

        [JsonPropertyName("emergencyLvl")]
        public EmergencyLvl? EmergencyLvl { get; set; }

        [JsonPropertyName("cardType")]
        public CardType? CardType { get; set; }

        [JsonPropertyName("category")]
        public Category? Category { get; set; }

        [JsonPropertyName("imgInfo")]
        public Image? ImgInfo { get; set; }
    }
}
