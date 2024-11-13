using DomainObjects.Subscription;
using Microsoft.Extensions.Options;
using PaymentService.Services.Interfaces;
using SubscriptionDBMongoAccessor;
using SubscriptionDBMongoAccessor.Infrastructure;

namespace PaymentService.Services.Implementations
{
    public class UsageService : IUsageService
    {
        private readonly SubscriptionDBAccessor _dbAccessor;

        public UsageService(IOptions<DbSettings> dbSettings, IOptions<SubscriptionsConfig> subscriptionsConfig)
        {
            _dbAccessor = new SubscriptionDBAccessor(dbSettings.Value, subscriptionsConfig.Value);
        }

        public async Task<Usage> DecrementUsageAsync(LimitKind limitKind, string userId)
        {
            var usage = await _dbAccessor.DecrementUsage(userId, limitKind);
            return usage;
        }

        public async Task<Usage> GetUsageAsync(string userId)
        {
            var usage = await _dbAccessor.GetUsage(userId);
            return usage;
        }
    }
}
