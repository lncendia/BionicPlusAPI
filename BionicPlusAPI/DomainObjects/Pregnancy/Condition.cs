using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class Condition
    {
        [JsonPropertyName("expression")]
        public List<Expression> Expressions { get; set; } = new List<Expression>();

        [JsonPropertyName("defaultCard")]
        public Annotation? DefaultCard { get; set; } 
    }
}
