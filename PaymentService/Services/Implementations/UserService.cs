using DomainObjects.Pregnancy.UserProfile;
using DomainObjects.Subscription;
using Microsoft.AspNetCore.Identity;
using PaymentService.Services.Interfaces;
using IdentityLibrary;
using Microsoft.Extensions.Options;
using PaymentService.Extensions;
using PaymentService.Models;

namespace PaymentService.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EncryptionConfig _encryptionConfig;
        
        public UserService(UserManager<ApplicationUser> userManager, IOptions<EncryptionConfig> encryptionConfig)
        {
            _userManager = userManager;
            _encryptionConfig = encryptionConfig.Value;
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public string GenerateUserIdHash(string userId)
        {
            // Generating HMAC for verification
            return userId.ComputeHmac(_encryptionConfig.UserIdSigningKey);
        }
        
        public bool VerifyUserIdHash(string userId, string hash)
        {
            // Generating HMAC for verification
            var computedHash = GenerateUserIdHash(userId);

            // Hash Comparison
            return computedHash.Equals(hash);
        }

        public async Task<string?> GetActiveSubscription(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException($"Not found user with id = {userId}");
            }

            var activeSubId = user.BillingProfile?.ActiveSubscriptionId;

            return activeSubId;
        }

        public async Task<bool> SaveUserAgreements(UserAgreement userAgreement, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            };

            if (user.UserAgreement.WhenOccured == null)
            {
                userAgreement.WhenOccured = DateTime.UtcNow;
                user.UserAgreement = userAgreement;
                await _userManager.UpdateAsync(user);
            }

            return true;
        }

        public async Task<bool> SetSubscription(string userId, string subscriptionId, bool isFreePlan)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            };

            user.BillingProfile ??= new BillingProfile();

            user.BillingProfile.ActiveSubscriptionId = subscriptionId;
            user.BillingProfile.isFreePlan = isFreePlan;
            user.BillingProfile.SubscriptionIds.Add(subscriptionId);

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
