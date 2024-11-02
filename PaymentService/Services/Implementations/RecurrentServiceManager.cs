﻿using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Implementations
{
    public class RecurrentServiceManager : IRecurrentServiceManager
    {
        private readonly ChargeRecurringService _chargeService;
        private readonly MailRecurringService _mailService;
        private readonly UsageRecurringService _usageService;

        public RecurrentServiceManager(ChargeRecurringService chargeService, MailRecurringService mailService, UsageRecurringService usageService)
        {
            _chargeService = chargeService;
            _mailService = mailService;
            _usageService = usageService;
        }

        public bool CancelAllRecurrentJobByUserId(string userId)
        {
            CancelRecurringPaymentsJobByUserId(userId);
            
            _usageService.CancelMonthlyJob(userId);
            _usageService.CancelYearlyJob(userId);

            return true;
        }

        public bool CancelRecurringPaymentsJobByUserId(string userId)
        {
            _chargeService.CancelMonthlyJob(userId);
            _mailService.CancelMounthlyJob(userId);
            
            _chargeService.CancelYearlyJob(userId);
            _mailService.CancelYearlyJob(userId);

            return true;
        }
    }
}
