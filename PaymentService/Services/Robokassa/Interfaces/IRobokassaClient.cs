namespace PaymentService.Services.Robokassa.Interfaces;

public interface IRobokassaClient
{
    Task<(string link, string subscriptionId)> GetCheckoutLink(string planId, string promocode);
}