using System.Globalization;
using DomainObjects.Subscription;
using PaymentService.Services.Interfaces;
using PaymentService.Models.Emails;

namespace PaymentService.Services.Implementations;

public class PaymentProcessorService
{
    private readonly MailRecurringService _mailService;
    private readonly IRobokassaMailService _robokassaMailService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;
    private readonly ChargeRecurringService _chargeService;
    private readonly IPlanService _planService;
    private readonly UsageRecurringService _usageService;
    private readonly ILogger<PaymentProcessorService> _logger;

    public PaymentProcessorService(MailRecurringService mailRecurringService,
        IRobokassaMailService robokassaMailService,
        ISubscriptionService subscriptionService,
        IUserService userService,
        ChargeRecurringService chargeRecurringService,
        IPlanService planService,
        UsageRecurringService usageService,
        ILogger<PaymentProcessorService> logger)
    {
        _mailService = mailRecurringService;
        _robokassaMailService = robokassaMailService;
        _subscriptionService = subscriptionService;
        _userService = userService;
        _chargeService = chargeRecurringService;
        _planService = planService;
        _usageService = usageService;
        _logger = logger;
    }

    public async Task<bool> ProcessSuccessPayment(string userId, string invoiceId, bool isFirst, string subscriptionId)
    {
        try
        {
            var subscription = await _subscriptionService.GetSubscription(subscriptionId);

            if(subscription.Status == SubscriptionStatus.Active)
            {
                return true;
            }

            var user = await _userService.GetUserById(userId);

            var userEmail = user.Email;

            var planId = subscription.PlanId;

            var plan = await _planService.GetPlan(planId);

            var successEmail = new SuccessPaymentEmailModel
            {
                UserId = userId,
                Hash = _userService.GenerateUserIdHash(userId),
                Email = userEmail,
                SubStartDate = subscription.CreationDate,
                SubEndDate = subscription.ExpirationDate,
                SubName = plan.Name
            };
                
            await _robokassaMailService.SendSuccessPaymentEmail(successEmail);

            await _subscriptionService.ActivateSubscription(subscriptionId);

            await _subscriptionService.SetSubscription(userId, planId, subscriptionId);

            if (!isFirst) return true;
                
            var recurrentPaymentEmail = new RecurrentPaymentEmailModel
            {
                UserId = userId,
                Hash = _userService.GenerateUserIdHash(userId),
                Email = userEmail,
                NextSubDate = subscription.ExpirationDate,
                Sum = plan.Price.ToString(CultureInfo.InvariantCulture),
                SubName = plan.Name,
            };
                
            // Recurrent notification about pay
            _mailService.PlanMonthlyPaymentNotificationMail(recurrentPaymentEmail);

            if (plan == null)
            {
                throw new ArgumentException($"Plan with id {planId} not found");
            }
                
            var isMonthly = plan.BillingPeriod == BillingPeriod.m;

            // Recurring charge
            if (isMonthly)
            {
                _chargeService.PlanMonthlyCharge(userId, planId, invoiceId);
            }
            else
            {
                _chargeService.PlanYearlyCharge(userId, planId, invoiceId);
            }

            // Recurring usage
            if (isMonthly)
            {
                _usageService.PlanMonthlyRefill(userId, planId);
            }
            else
            {
                _usageService.PlanYearlyRefill(userId, planId);
            }

            return true;
        }
        catch(Exception ex)
        {
            _logger.LogError($"Success processing error reason: {ex}");
            return false;
        }
    }
}