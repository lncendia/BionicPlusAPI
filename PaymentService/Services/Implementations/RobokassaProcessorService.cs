using DomainObjects.Subscription;
using IdentityLibrary;
using Microsoft.AspNetCore.Identity;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Implementations;

/// <summary>
/// The class of processing payments received from Robokassa
/// </summary>
public class RobokassaProcessorService: ISubscriptionProcessorService
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly MailRecurringService _mailService;
    private readonly ILogger<RobokassaProcessorService> _logger;
    private readonly ChargeRecurringService _chargeService;
    private readonly UsageRecurringService _usageService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentMailService _paymentMailService;
    private readonly IPlanService _planService;
    
    public RobokassaProcessorService(ISubscriptionService subscriptionService, UserManager<ApplicationUser> userManager,
        IPaymentMailService paymentMailService, IPlanService planService,
        ILogger<RobokassaProcessorService> logger, MailRecurringService mailService,
        ChargeRecurringService chargeService, UsageRecurringService usageService)
    {
        _subscriptionService = subscriptionService;
        _userManager = userManager; 
        _logger = logger;
        _paymentMailService = paymentMailService;
        _planService = planService;
        _mailService = mailService;
        _chargeService = chargeService;
        _usageService = usageService;
    }
    
    /// <summary>
    /// Processing the first subscription payment
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="invoiceId"></param>
    /// <param name="subscriptionId"></param>
    /// <returns>Processing result</returns>
    public async Task<bool> ProcessInitialPayment(string userId, string invoiceId, string subscriptionId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        var subscription = await _subscriptionService.GetSubscription(subscriptionId);

        var plan = await _planService.GetPlan(subscription.PlanId);
        
        if(subscription.Status == SubscriptionStatus.ACTIVE)
        {
            return true;
        }
        
        var processResult = await ProcessRenewalPayment(user, subscription, plan);

        if (!processResult) return false;
        
        _mailService.PlanMountlyPaymentNotificationMail(user.Email, userId, subscription.ExpirationDate, plan.Price.ToString(), plan.Name);
        
        switch (plan.BillingPeriod)
        {
            case BillingPeriod.m:
                _chargeService.PlanMountlyCharge(userId, plan.Id, invoiceId);
                _usageService.PlanMountlyRefill(userId, plan.Id);
                break;
            
            case BillingPeriod.y:
                _chargeService.PlanYearlyCharge(userId, plan.Id, invoiceId);
                _usageService.PlanYearlyRefill(userId, plan.Id);
                break;
            case BillingPeriod.d:
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return true;
    }

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

    private async Task<bool> ProcessRenewalPayment(ApplicationUser user, Subscription subscription, Plan plan)
    {
        await _paymentMailService.SendSuccessPaymentEmailAsync(user.Email, subscription.CreationDate, subscription.ExpirationDate, plan.Name);

        await _subscriptionService.ActivateSubscription(subscription.Id);

        await _subscriptionService.SetSubscription(user.Id.ToString(), plan.Id, subscription.Id);

        _logger.LogInformation("Subscription {subscriptionId} activated by user {userId}", subscription.Id, user.Id);
        
        return true;
    }
}