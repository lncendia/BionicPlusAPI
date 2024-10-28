using DomainObjects.Pregnancy.UserProfile;

namespace PaymentService.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> SaveUserAgreements(UserAgreement userAgreement, string userId);
        Task<bool> SetSubscription(string userId, string subscriptionId, bool isFreePlan);
        Task<string?> GetActiveSubscription(string userId);
    }
}
