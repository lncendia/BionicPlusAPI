using DomainObjects.Pregnancy.Localizations;
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
    public class Survey : SurveyBase
    {
        [JsonPropertyName("answers")]
        public List<SurveyAnswer>? Answers { get; set; }
    }
}
