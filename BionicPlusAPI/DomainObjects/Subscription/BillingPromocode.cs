using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Subscription
{
    public sealed class BillingPromocode
    {
        [JsonPropertyName("promocode")]
        public string? Promocode { get; set; }

        [JsonPropertyName("discountType")]
        public BillingPromocodeDiscountType? DiscountType { get; set; }

        [JsonPropertyName("discountValue")]
        public double? DiscountValue { get; set; }

    }
}
