using DomainObjects.Subscription;

namespace PaymentService.Services.Interfaces
{
    public interface IUsageService
    {
        Task<Usage> GetUsageAsync(string userId);

        Task<Usage> DecrementUsageAsync(LimitKind limitKind, string userId);
    }
}
