using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class ExtendedSurvey : Survey
    {
        [JsonPropertyName("answers")]
        public new List<ExtendedSurveyAnswer>? Answers { get; set; }
    }
}
