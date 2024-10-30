namespace PaymentService.Models;

public class CreateSubscriptionModel
{
    public string PlanId { get; set; } = string.Empty;
    public string? InvoiceId {get; set; }
    public string? FirstSubscriptionId {get; set; }
    public string? TransactionId { get; set; }
    public string? Promocode { get; set; }
}