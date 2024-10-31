﻿using DomainObjects.Subscription;
using Hangfire;
using PaymentService.Models;

namespace PaymentService.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<string> CreateFreeSubscription(string userId, bool setupUsage, bool discardUsage = true);
        Task<Subscription> GetSubscription(string subId);
        Task<BillingPromocode> GetPromocode(string promocode);
        Task<PriceModel> CalculatePrice(string planId, string promocode);
        Task<string> SetSubscription(string userId, string planId, string subscriptionId, bool isFreePlan = false);
        Task<string> CreateSubscription(CreateSubscriptionModel model);
        Task<string> ActivateSubscription(string subscriptionId);
        Task<string> DeactivateSubscription(string subscriptionId);
        Task<string> CancelSubscription(string userId);
        Task<bool> CheckInvoiceExist(int invoiceId);
        
        [Queue("usages")]
        Task InsureSubscription(string userId);
    }
}
