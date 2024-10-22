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

        public PlanService(IOptions<DbSettings> settings)
        {
            _dbAccessor = new SubscriptionDBAccessor(settings.Value);
        }
        public async Task<Plan> GetPlan(string planId)
        {
            var plan = await _dbAccessor.GetPlan(planId);
            return plan;
        }
    }
}
