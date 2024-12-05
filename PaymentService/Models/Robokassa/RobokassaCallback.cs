namespace PaymentService.Models.Robokassa;

public class RobokassaCallback
{
    public string InvoiceId { get; init; } = string.Empty;
    public string SubscriptionId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Signature { get; init; } = string.Empty;
    public string OutSum { get; init; } = string.Empty;
    public bool IsFirst { get; init; }
}