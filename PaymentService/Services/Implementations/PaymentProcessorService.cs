using System.Globalization;
using DomainObjects.Subscription;
using Microsoft.AspNetCore.Identity;
using PaymentService.Services.Interfaces;
using IdentityLibrary;
using PaymentService.Models.Emails;

namespace PaymentService.Services.Implementations
{
    public class PaymentProcessorService
    {
        private readonly MailRecurringService _mailService;
        private readonly IRobokassaMailService _robokassaMailService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ChargeRecurringService _chargeService;
        private readonly IPlanService _planService;
        private readonly UsageRecurringService _usageService;
        private readonly ILogger<PaymentProcessorService> _logger;

        public PaymentProcessorService(MailRecurringService mailRecurringService,
            IRobokassaMailService robokassaMailService,
            ISubscriptionService subscriptionService,
            UserManager<ApplicationUser> userManager,
            ChargeRecurringService chargeRecurringService,
            IPlanService planService,
            UsageRecurringService usageService,
            ILogger<PaymentProcessorService> logger)
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

        public async Task<bool> ProcessSuccessPayment(string userId, string invoiceId, bool isFirst, string subscriptionId)
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscription(subscriptionId);

                if(subscription.Status == SubscriptionStatus.Active)
                {
                    return true;
                }

                var user = await _userManager.FindByIdAsync(userId);

                var userEmail = user.Email;

                var planId = subscription.PlanId;

                var plan = await _planService.GetPlan(planId);

                var successEmail = new SuccessPaymentEmailModel()
                {
                    Email = userEmail,
                    SubStartDate = subscription.CreationDate,
                    SubEndDate = subscription.ExpirationDate,
                    SubName = plan.Name
                };
                
                await _robokassaMailService.SendSuccessPaymentEmail(successEmail);

                await _subscriptionService.ActivateSubscription(subscriptionId);

                await _subscriptionService.SetSubscription(userId, planId, subscriptionId);

                if (!isFirst) return true;
                
                //Recurrent notification about pay
                _mailService.PlanMonthlyPaymentNotificationMail(userEmail, userId, subscription.ExpirationDate, plan.Price.ToString(CultureInfo.InvariantCulture), plan.Name);

                if (plan == null)
                {
                    throw new ArgumentException($"Plan with id {planId} not found");
                }
                
                var isMonthly = plan.BillingPeriod == BillingPeriod.m;

                //Recurring charge
                if (isMonthly)
                {
                    _chargeService.PlanMountlyCharge(userId, planId, invoiceId);
                }
                else
                {
                    _chargeService.PlanYearlyCharge(userId, planId, invoiceId);
                }

                //Recurring usage
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
}
