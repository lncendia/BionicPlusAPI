namespace PaymentService.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<(string link, string subscriptionId)> GetCheckoutLink(string planId, string promocode);
    }
}
