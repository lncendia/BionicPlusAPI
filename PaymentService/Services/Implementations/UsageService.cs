using DomainObjects.Subscription;
using Microsoft.Extensions.Options;
using PaymentService.Models;
using PaymentService.Services.Interfaces;
using SubscriptionDBMongoAccessor;
using SubscriptionDBMongoAccessor.Infrastracture;

namespace PaymentService.Services.Implementations
{
    public class UsageService : IUsageService
    {
        private readonly SubscriptionDBAccessor _dbAccessor;

        public UsageService(IOptions<DbSettings> settings)
        {
            _dbAccessor = new SubscriptionDBAccessor(settings.Value);
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
