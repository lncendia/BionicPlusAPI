using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Subscription
{
    public class PriceModel
    {
        [JsonPropertyName("promocode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Promocode { get; set; }

        [JsonPropertyName("discountType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BillingPromocodeDiscountType? DiscountType { get; set; }

        [JsonPropertyName("discountValue")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? DiscountValue { get; set; }

        [JsonPropertyName("amountTotal")]
        public double? AmountTotal { get; set; }

        [JsonPropertyName("сurrency")]
        public Currency? Currency { get; set; }

        [JsonPropertyName("discountCurrencyValue")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? DiscountCurrencyValue { get; set; }
    }
}
