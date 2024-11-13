namespace PaymentService.Models.Emails;

public class FailedPaymentEmailModel: PaymentEmailModel
{
    public string SubName { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Discount { get; set; } = string.Empty;
    public string TotalPrice { get; set; } = string.Empty;
    public DateTime TryDate { get; set; }
}