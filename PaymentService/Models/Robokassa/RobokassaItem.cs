using System.Text.Json.Serialization;

namespace PaymentService.Models.Robokassa
{
    public class RobokassaItem
    {
        [JsonPropertyName("sum")]
        public decimal Sum { get; private set; }

        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; private set; }

        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; private set; } = "full_payment";

        [JsonPropertyName("payment_object")]
        public string PaymentObject { get; private set; } = "service";

        [JsonPropertyName("tax")]
        public string Tax { get; private set; } = "none";
        
        public RobokassaItem(decimal sum, string name, int quantity)
        {
            Sum = sum;
            Name = name;
            Quantity = quantity;
        }
    }
}
