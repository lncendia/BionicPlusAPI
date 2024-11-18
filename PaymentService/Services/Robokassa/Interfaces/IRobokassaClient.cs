namespace PaymentService.Services.Robokassa.Interfaces;

public interface IRobokassaClient
{
    Task<(string link, string subscriptionId)> GetCheckoutLink(string planId, string? promocode);

    Task<bool> ChargeRecurrentPayment(string previousInvoiceId, string planId, string userId);

    bool VerifySignature(string signature, string outSum, string invId, string userId, bool isFirst,
        string subscriptionId);
}