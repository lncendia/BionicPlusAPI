using DomainObjects.Pregnancy.UserProfile;
using IdentityLibrary;

namespace PaymentService.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> SaveUserAgreements(UserAgreement userAgreement, string userId);
        Task<bool> SetSubscription(string userId, string subscriptionId, bool isFreePlan);
        Task<string?> GetActiveSubscription(string userId);
        Task<ApplicationUser> GetUserById(string id);
        string GenerateUserIdHash(string userId);
        bool VerifyUserIdHash(string userId, string hash);
    }
}
