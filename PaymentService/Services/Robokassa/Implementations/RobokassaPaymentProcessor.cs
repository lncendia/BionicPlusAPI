using DomainObjects.Subscription;
using IdentityLibrary;
using Microsoft.AspNetCore.Identity;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Robokassa.Implementations.Recurring;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.Robokassa.Implementations;

public class RobokassaPaymentProcessor : IPaymentProcessor
{
    private readonly MailRecurringService _mailService;
    private readonly IRobokassaMailService _robokassaMailService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ChargeRecurringService _chargeService;
    private readonly IPlanService _planService;
    private readonly UsageRecurringService _usageService;
    private readonly ILogger<RobokassaPaymentProcessor> _logger;

    public RobokassaPaymentProcessor(MailRecurringService mailRecurringService,
        IRobokassaMailService robokassaMailService,
        ISubscriptionService subscriptionService,
        UserManager<ApplicationUser> userManager,
        ChargeRecurringService chargeRecurringService,
        IPlanService planService,
        UsageRecurringService usageService,
        ILogger<RobokassaPaymentProcessor> logger)
    {
        _mailService = mailRecurringService;
        _robokassaMailService = robokassaMailService;
        _subscriptionService = subscriptionService;
        _userManager = userManager;
        _chargeService = chargeRecurringService;
        _planService = planService;
        _usageService = usageService;
        _logger = logger;
    }

    public async Task<bool> ProcessAsync(string invoiceId, bool isFirst, string subscriptionId)
    {
        try
        {
            await _robokassaMailService.SendSuccessPaymentEmailAsync(userEmail, subscription.CreationDate, subscription.ExpirationDate, plan.Name);

            if (!isFirst) return true;
                
            //Recurrent notification about pay
            _mailService.PlanMountlyPaymentNotificationMail(userEmail, userId, subscription.ExpirationDate, plan.Price.ToString(), plan.Name);
            
            var isMountly = plan.BillingPeriod == BillingPeriod.m;

            //Recurring charge
            if (isMountly)
            {
                _chargeService.PlanMountlyCharge(userId, planId, invoiceId);
            }
            else
            {
                _chargeService.PlanYearlyCharge(userId, planId, invoiceId);
            }

            //Recurring usage
            if (isMountly)
            {
                _usageService.PlanMountlyRefill(userId, planId);
            }
            else
            {
                _usageService.PlanYearlyRefill(userId, planId);
            }

            return true;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Success processing error");
            return false;
        }
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