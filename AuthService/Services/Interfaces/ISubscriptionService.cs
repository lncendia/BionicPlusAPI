using DomainObjects.Subscription;

namespace AuthService.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<string?> SetFreeSubscription(string userId);
        Task<Subscription?> GetSubscription(string subscriptionId);
        Task<bool> CancelUserSubscription();
        Task<Plan?> GetPlan(string planId);
        Task<Usage?> GetUsage(string userId);
    }
}
