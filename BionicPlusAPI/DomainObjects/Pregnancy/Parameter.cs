using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class Parameter
    {
        [JsonPropertyName("parameterType")]
        public ParameterType ParameterType { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("expression")]
        public Expression? Expression { get; set; } 
    }
}
