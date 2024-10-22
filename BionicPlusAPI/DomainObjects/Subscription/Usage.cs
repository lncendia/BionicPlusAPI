using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Subscription
{
    public class Usage
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("surveyUsage")]
        public int SurveyUsage { get; set; }

        [JsonPropertyName("rollbackUsage")]
        public int RollbackUsage { get; set; }
    }
}
