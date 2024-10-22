using DomainObjects.Subscription;

namespace BionicPlusAPI.Services.Interfaces
{
    public interface IUsageService
    {
        Task<Usage> GetUsageAsync(string userId);

        Task<Usage> DecrementUsageAsync(LimitKind limitKind, string userId);
    }
}
