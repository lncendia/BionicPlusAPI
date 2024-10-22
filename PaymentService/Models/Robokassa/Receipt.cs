using System.Text.Json.Serialization;

namespace PaymentService.Models.Robokassa
{
    public class Receipt
    {
        [JsonPropertyName("sno")]
        public string Sno { get; private set; } = "usn_income";

        [JsonPropertyName("items")]
        public List<RobokassaItem> Items { get; private set; }

        public Receipt(List<RobokassaItem> items)
        {
            Items = items;
        }
    }
}
