using DomainObjects.Subscription;
using Microsoft.Extensions.Options;
using PaymentService.Services.Interfaces;
using SubscriptionDBMongoAccessor;
using SubscriptionDBMongoAccessor.Infrastracture;

namespace PaymentService.Services.Implementations
{
    public class PlanService : IPlanService
    {
        private readonly SubscriptionDBAccessor _dbAccessor;

        public PlanService(IOptions<DbSettings> dbSettings, IOptions<SubscriptionsConfig> subscriptionsConfig)
        {
            _dbAccessor = new SubscriptionDBAccessor(dbSettings.Value, subscriptionsConfig.Value);
        }
        
        public async Task<Plan> GetPlan(string planId)
        {
            var plan = await _dbAccessor.GetPlan(planId);
            return plan;
        }
    }
}
