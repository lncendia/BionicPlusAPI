using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Subscription
{
    public class Limit
    {
        [JsonPropertyName("surveyLimit")]
        public int SurveyLimit { get; set; }

        [JsonPropertyName("rollbackLimit")]
        public int RollbackLimit { get; set; }
    }
}
