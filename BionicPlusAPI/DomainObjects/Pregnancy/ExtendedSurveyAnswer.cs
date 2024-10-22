using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class ExtendedSurveyAnswer : SurveyAnswer
    {
        [JsonPropertyName("card")]
        public Annotation? Card { get; set; }

        [JsonPropertyName("answerTitle")]
        public string? AnswerTitle { get; set; }

        [JsonPropertyName("imageInfo")]
        public Image? ImageInfo { get; set; }
    }
}
