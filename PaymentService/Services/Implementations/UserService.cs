using DomainObjects.Pregnancy.UserProfile;
using DomainObjects.Subscription;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PaymentService.Models;
using PaymentService.Services.Interfaces;
using System.Text;
using System.Text.Json;
using IdentityLibrary;

namespace PaymentService.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

            if(user.BillingProfile == null)
            {
                user.BillingProfile = new BillingProfile();
            }

            user.BillingProfile.ActiveSubscriptionId = subscriptionId;
            user.BillingProfile.isFreePlan = isFreePlan;
            user.BillingProfile.SubscriptionIds.Add(subscriptionId);

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
