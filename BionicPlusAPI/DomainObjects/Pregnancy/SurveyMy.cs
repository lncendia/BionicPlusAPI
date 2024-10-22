using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class SurveyMy : SurveyBase
    {
        [JsonPropertyName("lastAnswer")]
        public ExtendedSurveyAnswer? Answer { get; set; }
    }
}
