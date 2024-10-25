using Hangfire;

namespace PaymentService.Services.Implementations
{
    public class ChargeRecurringService
    {
        private const string MonthlyPrefix = "MounthlyChargeJob";
        private const string YearlyPrefix = "YearlyChargeJob";

        public void PlanMonthlyCharge(string userId, string planId, string firstInvoiceId)
        {
            var utcNow = DateTime.UtcNow.AddHours(-2);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<RobokassaService>($"{MonthlyPrefix}_{userId}", x => x.ChargeRecurrentPayment(firstInvoiceId, planId, userId), Cron.Monthly(day, hours, minutes));
        }

        public void PlanYearlyCharge(string userId, string planId, string firstInvoiceId)
        {
            var utcNow = DateTime.UtcNow.AddHours(-2);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<RobokassaService>($"{YearlyPrefix}_{userId}", x => x.ChargeRecurrentPayment(firstInvoiceId, planId, userId), Cron.Yearly(day, hours, minutes));
        }

        public bool FindJobById(string userId, bool isMountly)
        {
            using var connection = JobStorage.Current.GetConnection();
            var job = connection.GetJobData(isMountly ? $"{MonthlyPrefix}_{userId}" : $"{YearlyPrefix}_{userId}");

            return job != null;
        }

        public void CancelMonthlyJob(string userId)
        {
            RecurringJob.RemoveIfExists($"{MonthlyPrefix}_{userId}");
        }

        public void CancelYearlyJob(string userId)
        {
            RecurringJob.RemoveIfExists($"{YearlyPrefix}_{userId}");
        }
    }
}
