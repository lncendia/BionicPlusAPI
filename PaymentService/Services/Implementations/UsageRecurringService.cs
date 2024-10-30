using Hangfire.Storage;
using Hangfire;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Implementations
{
    public class UsageRecurringService
    {
        private const string MOUNTHLY_PREFIX = "MounthlyUsageJob";
        private const string YEARLY_PREFIX = "YearlyUsageJob";
        private readonly ILogger<UsageRecurringService> _logger;

        public UsageRecurringService(ILogger<UsageRecurringService> logger)
        {
            _logger = logger;
        }
        public void PlanMountlyRefill(string userId, string planId)
        {
            var utcNow = DateTime.UtcNow;

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<ISubscriptionService>($"{MOUNTHLY_PREFIX}_{userId}", x => x.InsureSubscription(userId), Cron.Monthly(day, hours, minutes));
        }

        public void PlanYearlyRefill(string userId, string planId)
        {
            var utcNow = DateTime.UtcNow;

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<ISubscriptionService>($"{YEARLY_PREFIX}_{userId}", x => x.InsureSubscription(userId), Cron.Yearly(day, hours, minutes));
        }

        public bool FindJobById(string userId, bool isMountly)
        {
            using (IStorageConnection connection = JobStorage.Current.GetConnection())
            {
                JobData? job = default;
                if (isMountly)
                {
                    job = connection.GetJobData($"{MOUNTHLY_PREFIX}_{userId}");
                }
                else
                {
                    job = connection.GetJobData($"{YEARLY_PREFIX}_{userId}");
                }

                if (job == null)
                {
                    return false;
                }

                return true;
            }
        }

        public void CancelMounthlyJob(string userId)
        {
            RecurringJob.RemoveIfExists($"{MOUNTHLY_PREFIX}_{userId}");
        }

        public void CancelYearlyJob(string userId)
        {
            RecurringJob.RemoveIfExists($"{YEARLY_PREFIX}_{userId}");
        }
    }
}
