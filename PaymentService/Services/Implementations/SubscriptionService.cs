﻿using DomainObjects.Subscription;
using Hangfire;
using Microsoft.Extensions.Options;
using PaymentService.Models;
using PaymentService.Services.Interfaces;
using SubscriptionDBMongoAccessor;
using SubscriptionDBMongoAccessor.Infrastracture;

namespace PaymentService.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionDBAccessor _dbAccessor;
        private readonly PlansConfiguration _plans;
        private readonly IUserService _userService;

        public SubscriptionService(IOptions<DbSettings> settings, IOptions<PlansConfiguration> plans, IUserService userService)
        {
            _dbAccessor = new SubscriptionDBAccessor(settings.Value);
            _plans = plans.Value;
            _userService = userService;
        }

        public Task<Subscription> GetSubscription(string subId)
        {
            var subscription = _dbAccessor.GetSubscription(subId);
            return subscription;
        }

        public async Task<string> SetFreeSubscription(string userId, bool setupUsage, bool discardUsage = true)
        {
            var freePlanId = _plans.FreePlanId;

            // TODO: определять как отменять подписку пользователя в зависимости о её вида
            _recurrentServiceManager.CancelAllRecurrentJobByUserId(userId);

            if (setupUsage && discardUsage)
            {
                await _dbAccessor.SetUpUsage(userId, freePlanId);
            }
            else
            {
                await _dbAccessor.SetUsage(userId, freePlanId);
            }

            var subscriptionId = await _dbAccessor.SetFreeSubscription(userId, freePlanId);

            return subscriptionId;
        }

        public async Task<string> SetSubscription(string userId, string planId, string subscriptionId)
        {
            await _dbAccessor.SetUsage(userId, planId);
           
            var result = await _userService.SetSubscription(userId, subscriptionId, false);

            return subscriptionId;
        }

        public async Task<string> CreateSubscription(CreateSubscriptionModel model)
        {
            decimal sale = 0; 
            
            if (model.Promocode != null)
            {
                var promoModel = await _dbAccessor.GetPromocode(model.Promocode);
                var plan = await _dbAccessor.GetPlan(model.PlanId);
                sale = CalculateSale(promoModel, plan);
            }
            
            var subscriptionId = await _dbAccessor.CreateSubscription(model, sale);

            return subscriptionId;
        }

        [Queue("usages")]
        public async Task InsureSubscription(string userId)
        {
            var subId = await _userService.GetActiveSubscription(userId);

            if (subId == null)
            {
                return;
            }

            var subscription = await GetSubscription(subId);
            if (subscription.Status != SubscriptionStatus.ACTIVE)
            {
                await DeactivateSubscription(subId);
                var freeSubID = await SetFreeSubscription(userId, false);
                await _userService.SetSubscription(userId, freeSubID, true);
            }
        }

        public async Task<string> ActivateSubscription(string subscriptionId)
        {
            return await _dbAccessor.ActivateSubscription(subscriptionId);
        }

        public async Task<string> DeactivateSubscription(string subscriptionId)
        {
            return await _dbAccessor.ActivateSubscription(subscriptionId);
        }

        public async Task<bool> CheckInvoiceExist(int invoiceId)
        {
            return await _dbAccessor.CheckInvoiceExist(invoiceId);
        }
        
        public bool CancelAllUserRecurringJobs(string userId)
        {
            try
            {
                _recurrentServiceManager.CancelAllRecurrentJobByUserId(userId);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
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

            if(promoModel == null)
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

        private decimal CalculateSale(BillingPromocode? promocode, Plan plan)
        {
            decimal sale = 0;

            if(promocode == null)
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
}
