namespace PaymentService.Models;

public class CheckoutModel
{
    public string SubscriptionId { get; init; } = string.Empty;
    public string? Link { get; init; }
}