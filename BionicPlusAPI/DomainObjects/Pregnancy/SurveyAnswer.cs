using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class SurveyAnswer
    {
        [Required]
        [JsonPropertyName("cardId")]
        public string CardId { get; set; }

        [JsonPropertyName("answerId")]
        public Guid? AnswerId { get; set; }

        [JsonPropertyName("userInputType")]
        public UserInputType? UserInputType { get; set; }

        [JsonPropertyName("userInputValue")]
        public string? UserInputValue { get; set; }
    }
}
