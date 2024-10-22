using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy.Children
{
    public class Child
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }

        [JsonPropertyName("measurements")]
        public List<ChildMeasurement>? Measurements { get; set; }

        [JsonPropertyName("whenArchived")]
        public DateTime? WhenArchived { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
