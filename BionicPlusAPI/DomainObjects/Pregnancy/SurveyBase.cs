using DomainObjects.Pregnancy.Localizations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class SurveyBase
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [Required]
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [Required]
        [JsonPropertyName("type")]
        public SurveyType? Type { get; set; }

        [Required]
        [JsonPropertyName("currentCardId")]
        public string? CurrentCardId { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("deleteDate")]
        public DateTime? DeleteDate { get; set; }

        [Required]
        [JsonPropertyName("localization")]
        public LocalizationsLanguage? LocalizationLanguage { get; set; }

        [JsonPropertyName("rating")]
        public Rating? Rating { get; set; }
    }
}
