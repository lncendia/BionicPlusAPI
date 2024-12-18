﻿using DomainObjects.Subscription;
using Hangfire;
using Microsoft.Extensions.Options;
using PaymentService.Models;
using PaymentService.Services.Interfaces;
using SubscriptionDBMongoAccessor;
using SubscriptionDBMongoAccessor.Infrastructure;

namespace PaymentService.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly SubscriptionDBAccessor _dbAccessor;
    private readonly UsageRecurringService _usageRecurringService;
    private readonly PlansConfiguration _plans;
    private readonly IUserService _userService;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(IOptions<DbSettings> dbSettings, IOptions<SubscriptionsConfig> subscriptionsConfig,
        IOptions<PlansConfiguration> plans, IUserService userService, ILogger<SubscriptionService> logger,
        UsageRecurringService usageRecurringService)
    {
        _dbAccessor = new SubscriptionDBAccessor(dbSettings.Value, subscriptionsConfig.Value);
        _plans = plans.Value;
        _userService = userService;
        _logger = logger;
        _usageRecurringService = usageRecurringService;
    }

    public Task<Subscription> GetSubscription(string subId)
    {
        var subscription = _dbAccessor.GetSubscription(subId);
        return subscription;
    }

    public async Task<string> CreateFreeSubscription(string userId, bool setupUsage, bool discardUsage = true)
    {
        var freePlanId = _plans.FreePlanId;

        if (setupUsage && discardUsage)
        {
            await _dbAccessor.SetUpUsage(userId, freePlanId);
        }
        else
        {
            await _dbAccessor.SetUsage(userId, freePlanId);
        }

        var subscriptionId = await _dbAccessor.SetFreeSubscription(freePlanId);

        return subscriptionId;
    }

    public async Task<string> SetSubscription(string userId, string planId, string subscriptionId)
    {
        await _dbAccessor.SetUsage(userId, planId);

        var result = await _userService.SetSubscription(userId, subscriptionId, false);

        return subscriptionId;
    }

    public async Task<string> CreateSubscription(string planId, PaymentServiceType serviceType, string? invoiceId, string? promocode = null)
    {
        var subscriptionId = string.Empty;

        if (promocode != null)
        {
            var promoModel = await _dbAccessor.GetPromocode(promocode);
            var plan = await _dbAccessor.GetPlan(planId);
            var sale = CalculateSale(promoModel, plan);

            subscriptionId = await _dbAccessor.CreateSubscription(planId, serviceType, invoiceId, sale, promocode);
        }
        else
        {
            subscriptionId = await _dbAccessor.CreateSubscription(planId, serviceType, invoiceId, 0, promocode);
        }

        return subscriptionId;
    }

    public async Task SetGooglePurchaseToken(string subscriptionId, string orderId, string purchaseToken)
    {
        await _dbAccessor.SetGooglePurchaseToken(subscriptionId, orderId, purchaseToken);
    }

    public async Task<Subscription> GetSubscriptionByGoogleOrderId(string googleOrderId)
    {
        return await _dbAccessor.GetSubscriptionByOrderId(googleOrderId);
    }

    [Queue("usages")]
    public async Task InsureSubscription(string userId)
    {
        var subscriptionId = await _userService.GetActiveSubscription(userId);

        if (subscriptionId == null) return;

        var subscription = await GetSubscription(subscriptionId);
        if (subscription.Status != SubscriptionStatus.Active)
        {
            _usageRecurringService.CancelMonthlyJob(userId);
            _usageRecurringService.CancelYearlyJob(userId);
            var freeSubscriptionId = await CreateFreeSubscription(userId, false);
            await _userService.SetSubscription(userId, freeSubscriptionId, true);
        }
    }

    public async Task<string> ActivateSubscription(string subscriptionId)
    {
        return await _dbAccessor.SetSubscriptionStatus(subscriptionId, SubscriptionStatus.Active);
    }

    public async Task<string> DeactivateSubscription(string subscriptionId)
    {
        return await _dbAccessor.SetSubscriptionStatus(subscriptionId, SubscriptionStatus.Inactive);
    }

    public async Task<bool> CheckInvoiceExist(int invoiceId)
    {
        return await _dbAccessor.CheckInvoiceExist(invoiceId);
    }

    public async Task<BillingPromocode> GetPromocode(string promocode)
    {
        var promoModel = await _dbAccessor.GetPromocode(promocode);

        if (promoModel == null)
        {
            throw new ArgumentException($"Not found promocode {promocode}");
        }

        return promoModel;
    }

    public async Task<PriceModel> CalculatePrice(string planId, string promocode)
    {
        var promoModel = await _dbAccessor.GetPromocode(promocode);

        var plan = await _dbAccessor.GetPlan(planId);

        var sale = CalculateSale(promoModel, plan);

        if (promoModel == null)
        {
            return new PriceModel
            {
                Currency = plan.Currency,
                AmountTotal = (double)plan.Price
            };
        }

        return new PriceModel
        {
            Promocode = promoModel.Promocode,
            AmountTotal = (double)plan.Price - (double)sale,
            DiscountType = promoModel.DiscountType,
            DiscountValue = promoModel.DiscountValue,
            Currency = plan.Currency,
            DiscountCurrencyValue = (double)sale,
        };
    }

    [Queue("expired_subscriptions")]
    public async Task CancelExpiredSubscriptions()
    {
        var updateSubscriptionsCount = await _dbAccessor.UpdateExpiredPendingSubscriptionsToFailed();

        if (updateSubscriptionsCount > 0)
        {
            _logger.LogInformation("{count} subscriptions mark as failed after expired", updateSubscriptionsCount);
        }
    }

    private decimal CalculateSale(BillingPromocode? promocode, Plan plan)
    {
        decimal sale = 0;

        if (promocode == null)
        {
            return 0;
        }

        if (promocode.DiscountType == BillingPromocodeDiscountType.Percent)
        {
            sale = Math.Round(plan.Price / 100 * (decimal)promocode.DiscountValue!, 2);
        }
        else
        {
            sale = (decimal)promocode.DiscountValue!;
        }

        return sale;
    }
}