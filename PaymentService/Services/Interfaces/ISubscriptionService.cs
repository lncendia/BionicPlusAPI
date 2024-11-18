using DomainObjects.Subscription;
using Hangfire;

namespace PaymentService.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<string> CreateFreeSubscription(string userId, bool setupUsage, bool discardUsage = true);
        Task<Subscription> GetSubscription(string subId);
        Task<BillingPromocode> GetPromocode(string promocode);
        Task<PriceModel> CalculatePrice(string planId, string promocode);
        Task<string> SetSubscription(string userId, string planId, string subscriptionId);
        Task<string> CreateSubscription(string planId, PaymentServiceType serviceType, string invoiceId = "",
            string? promocode = null);
        Task<string> ActivateSubscription(string subscriptionId);
        Task<string> DeactivateSubscription(string subscriptionId);
        bool CancelPaymentReccuringJobs(string userId); // todo: выноситься в пеймент провайдер
        Task<bool> CheckInvoiceExist(int invoiceId);
        Task SetGooglePurchaseToken(string subscriptionId, string orderId, string purchaseToken);
        Task<Subscription> GetSubscriptionByGoogleOrderId(string googleOrderId);

        [Queue("usages")]
        Task InsureSubscription(string userId);
        
        [Queue("expired_subscriptions")]
        Task CancelExpiredSubscriptions();
    }
}
