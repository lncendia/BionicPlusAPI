using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class Expression
    {
        [JsonPropertyName("leftParameter")]
        public Parameter LeftParameter { get; set; } = new Parameter();

        [JsonPropertyName("rightParameter")]
        public Parameter RightParameter { get; set; } = new Parameter();

        [JsonPropertyName("operationType")]
        public OperationType OperationType { get; set; }

        [JsonPropertyName("card")]
        public Annotation? Card { get; set; }
    }
}
