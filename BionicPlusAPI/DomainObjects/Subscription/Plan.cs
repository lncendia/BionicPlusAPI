using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Subscription
{
    public class Plan
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("billingPeriod")]
        public BillingPeriod BillingPeriod { get; set; }

        [JsonPropertyName("billingUnit")]
        public int BillingUnit { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [JsonPropertyName("resellerId")]
        public string ResellerId { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; } 

        [JsonPropertyName("currency")]
        public Currency Currency { get; set; }

        [JsonPropertyName("limits")]
        public Limit? Limits { get; set; }
        
        [JsonPropertyName("googleSubscriptionId")]
        public string? GoogleSubscriptionId { get; set; } = string.Empty;
    }
}
