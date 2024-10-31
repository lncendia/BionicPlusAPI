using System.Globalization;
using DomainObjects.Subscription;
using IdentityLibrary;
using Microsoft.AspNetCore.Identity;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Robokassa.Implementations.Recurring;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.Robokassa.Implementations;

public class RobokassaPaymentProcessor : IPaymentProcessor
{
    private readonly IPaymentMailService _paymentMailService;
    private readonly MailRecurringService _mailService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ChargeRecurringService _chargeService;
    private readonly IPlanService _planService;
    private readonly UsageRecurringService _usageService;
    private readonly ILogger<RobokassaPaymentProcessor> _logger;

    public RobokassaPaymentProcessor(MailRecurringService mailRecurringService,
        ISubscriptionService subscriptionService,
        UserManager<ApplicationUser> userManager,
        ChargeRecurringService chargeRecurringService,
        IPlanService planService,
        UsageRecurringService usageService,
        ILogger<RobokassaPaymentProcessor> logger,
        IPaymentMailService paymentMailService)
    {
        _mailService = mailRecurringService;
        _subscriptionService = subscriptionService;
        _userManager = userManager;
        _chargeService = chargeRecurringService;
        _planService = planService;
        _usageService = usageService;
        _logger = logger;
        _paymentMailService = paymentMailService;
    }

    public async Task<bool> ProcessInitialAsync(string userId, string invoiceId, string subscriptionId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        var subscription = await _subscriptionService.GetSubscription(subscriptionId);

        var plan = await _planService.GetPlan(subscription.PlanId);
        
        if(subscription.Status == SubscriptionStatus.ACTIVE)
        {
            return true;
        }
        
        var processResult = await ProcessAsync(user, subscription, plan);

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

    
    public async Task<bool> ProcessAsync(string userId, string invoiceId, string subscriptionId)
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

    private async Task<bool> ProcessAsync(ApplicationUser user, Subscription subscription, Plan plan)
    {
        await _paymentMailService.SendSuccessPaymentEmailAsync(user.Email, subscription.CreationDate, subscription.ExpirationDate, plan.Name);

        await _subscriptionService.ActivateSubscription(subscription.Id);

        await _subscriptionService.SetSubscription(user.Id.ToString(), plan.Id, subscription.Id);

        _logger.LogInformation("Subscription {subscriptionId} activated by user {userId}", subscription.Id, user.Id);
        
        return true;
    }
    
    public Task CancelAsync(string userId)
    {
        _chargeService.CancelMounthlyJob(userId);
        _mailService.CancelMounthlyJob(userId);
        _usageService.CancelMounthlyJob(userId);

        _chargeService.CancelYearlyJob(userId);
        _mailService.CancelYearlyJob(userId);
        _usageService.CancelYearlyJob(userId);
    }
}