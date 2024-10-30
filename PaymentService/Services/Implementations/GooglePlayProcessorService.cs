using IdentityLibrary;
using Microsoft.AspNetCore.Identity;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Implementations;

/// <summary>
/// The class of processing payments received from GooglePlay
/// </summary>
public class GooglePlayProcessorService: ISubscriptionProcessorService
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly MailRecurringService _mailService;
    private readonly ILogger<RobokassaProcessorService> _logger;
    private readonly ChargeRecurringService _chargeService;
    private readonly UsageRecurringService _usageService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentMailService _paymentMailService;
    private readonly IPlanService _planService;
    
    /// <summary>
    /// Processing the subscription payment
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="invoiceId"></param>
    /// <param name="subscriptionId"></param>
    /// <returns>Processing result</returns>
    public async Task<bool> ProcessRenewalPayment(string userId, string invoiceId, string subscriptionId)
    {
        var subscription = await _subscriptionService.GetSubscription(subscriptionId);
        
        var user = await _userManager.FindByIdAsync(userId);
        
        var plan = await _planService.GetPlan(subscription.PlanId);

        if (plan.Id == null)
        {
            return false;
        }
        
        await _paymentMailService.SendSuccessPaymentEmailAsync(user.Email, subscription.CreationDate, subscription.ExpirationDate, plan.Name);

        await _subscriptionService.ActivateSubscription(subscriptionId);

        await _subscriptionService.SetSubscription(userId, plan.Id, subscriptionId);
        
        _logger.LogInformation("Subscription {subscriptionId} activated by user {userId}", subscriptionId, userId);

        return true;
    }

    /// <summary>
    /// Processing the subscription cancellation
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> ProcessCancellation(string userId)
    {
        await _subscriptionService.InsureSubscription(userId);
        return true;
    }
}