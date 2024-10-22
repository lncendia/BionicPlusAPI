using DomainObjects.Subscription;
using Hangfire;

namespace PaymentService.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<string> SetFreeSubscription(string userId, bool setupUsage, bool discardUsage = true);
        Task<Subscription> GetSubscription(string subId);
        Task<BillingPromocode> GetPromocode(string promocode);
        Task<PriceModel> CalculatePrice(string planId, string promocode);
        Task<string> SetSubscription(string userId, string planId, string subscriptionId);
        Task<string> CreateSubscription(string planId, string invoiceId, string? promocode = null);
        Task<string> ActivateSubscription(string subscriptionId);
        Task<string> DeactivateSubscription(string subscriptionId);
        bool CancelAllUserRecurringJobs(string userId);

        Task<bool> CheckInvoiceExist(int invoiceId);

        [Queue("usages")]
        Task InsureSubscription(string userId, string planId);
    }
}
