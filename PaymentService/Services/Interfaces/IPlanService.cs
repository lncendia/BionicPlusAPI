using DomainObjects.Subscription;

namespace PaymentService.Services.Interfaces
{
    public interface IPlanService
    {
        Task<Plan> GetPlan(string planId);
    }
}
