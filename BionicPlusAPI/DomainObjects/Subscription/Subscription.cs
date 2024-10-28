using System.Text.Json.Serialization;

namespace DomainObjects.Subscription
{
    public class Subscription
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [JsonPropertyName("limits")]
        public Limit? Limits { get; set; }

        [JsonPropertyName("invoiceId")]
        public int InvoiceId { get; set; } 

        [JsonPropertyName("status")]
        public SubscriptionStatus Status { get; set; } 

        [JsonPropertyName("planId")]
        public string PlanId { get; set; } = string.Empty;

        [JsonPropertyName("isRecurrent")]
        public bool isRecurrent { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("currency")]
        public Currency Currency { get; set; }

        [JsonPropertyName("discount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal Discount { get; set; }

        [JsonPropertyName("promocode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Promocode { get; set; }
    }
}
