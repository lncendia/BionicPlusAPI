using System.Globalization;
using DomainObjects.Subscription;
using Microsoft.Extensions.Options;
using PaymentService.Models;
using PaymentService.Models.Emails;
using PaymentService.Models.Robokassa;
using PaymentService.Services.Implementations;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Robokassa.Implementations.Recurring;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.Robokassa.Implementations;

public class RobokassaPaymentProcessor : IPaymentProcessor<RobokassaCallback>
{
    private readonly MailRecurringService _mailService;
    private readonly IPaymentMailService _paymentMailService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;
    private readonly ChargeRecurringService _chargeService;
    private readonly IPlanService _planService;
    private readonly UsageRecurringService _usageService;
    private readonly EndpointsConfig _endpointsConfig;
    private readonly IRobokassaClient _robokassaClient;

    public RobokassaPaymentProcessor(
        IRobokassaClient robokassaClient,
        MailRecurringService mailRecurringService,
        IPaymentMailService paymentMailService,
        ISubscriptionService subscriptionService,
        IUserService userService,
        ChargeRecurringService chargeRecurringService,
        IPlanService planService,
        UsageRecurringService usageService,
        IOptions<EndpointsConfig> endpointsConfig)
    {
        _robokassaClient = robokassaClient;
        _mailService = mailRecurringService;
        _paymentMailService = paymentMailService;
        _subscriptionService = subscriptionService;
        _userService = userService;
        _chargeService = chargeRecurringService;
        _planService = planService;
        _usageService = usageService;
        _endpointsConfig = endpointsConfig.Value;
    }

    public Task VerifyAsync(RobokassaCallback callback)
    {
        var result = _robokassaClient.VerifySignature(
            callback.Signature,
            callback.OutSum,
            callback.InvoiceId,
            callback.UserId,
            callback.IsFirst,
            callback.SubscriptionId);

        return Task.FromResult(result);
    }

    public async Task ProcessAsync(RobokassaCallback callback)
    {
        var subscription = await _subscriptionService.GetSubscription(callback.SubscriptionId);

        if (subscription.Status == SubscriptionStatus.Active) return;

        var planId = subscription.PlanId;

        var plan = await _planService.GetPlan(planId);

        var successEmail = new SuccessPaymentEmailModel
        {
            CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
            UserId = callback.UserId,
            Hash = _userService.GenerateUserIdHash(callback.UserId),
            Email = callback.Email,
            SubStartDate = subscription.CreationDate,
            SubEndDate = subscription.ExpirationDate,
            SubName = plan.Name
        };

        await _paymentMailService.SendSuccessPaymentEmail(successEmail);

        await _subscriptionService.ActivateSubscription(callback.SubscriptionId);

        await _subscriptionService.SetSubscription(callback.UserId, planId, callback.SubscriptionId);

        if (!callback.IsFirst) return;

        var recurrentPaymentEmail = new RecurrentPaymentEmailModel
        {
            CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
            UserId = callback.UserId,
            Hash = _userService.GenerateUserIdHash(callback.UserId),
            Email = callback.Email,
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

        // Recurring charge
        if (plan.BillingPeriod == BillingPeriod.m)
        {
            _chargeService.PlanMonthlyCharge(callback.UserId, planId, callback.InvoiceId);
            _usageService.PlanMonthlyRefill(callback.UserId, planId);
        }
        else
        {
            _chargeService.PlanYearlyCharge(callback.UserId, planId, callback.InvoiceId);
            _usageService.PlanYearlyRefill(callback.UserId, planId);
        }
    }

    public Task CancelAsync(string userId)
    {
        _chargeService.CancelMonthlyJob(userId);
        _mailService.CancelMounthlyJob(userId);

        _chargeService.CancelYearlyJob(userId);
        _mailService.CancelYearlyJob(userId);

        return Task.CompletedTask;
    }
}