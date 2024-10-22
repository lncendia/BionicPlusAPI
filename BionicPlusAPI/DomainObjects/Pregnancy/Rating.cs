using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public class Rating
    {
        [JsonPropertyName("description")]
        [StringLength(maximumLength: 200)]
        public string? Description { get; set; }

        [JsonPropertyName("score")]
        public int? Score { get; set; }
    }
}
