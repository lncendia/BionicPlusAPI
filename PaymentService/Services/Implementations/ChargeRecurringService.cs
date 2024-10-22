using Hangfire;
using Hangfire.Storage;

namespace PaymentService.Services.Implementations
{
    public class ChargeRecurringService
    {
        private const string MOUNTHLY_PREFIX = "MounthlyChargeJob";
        private const string YEARLY_PREFIX = "YearlyChargeJob";

        public void PlanMountlyCharge(string userId, string planId, string firstInvoiceId)
        {
            var utcNow = DateTime.UtcNow.AddHours(-2);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<RobokassaService>($"{MOUNTHLY_PREFIX}_{userId}", x => x.ChargeRecurrentPayment(firstInvoiceId, planId, userId), Cron.Monthly(day, hours, minutes));
        }

        public void PlanYearlyCharge(string userId, string planId, string firstInvoiceId)
        {
            var utcNow = DateTime.UtcNow.AddHours(-2);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<RobokassaService>($"{YEARLY_PREFIX}_{userId}", x => x.ChargeRecurrentPayment(firstInvoiceId, planId, userId), Cron.Yearly(day, hours, minutes));
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
