using Hangfire;

namespace PaymentService.Services.Implementations
{
    public class MailRecurringService 
    {
        private const string MOUNTHLY_PREFIX = "MounthlyEmailJob";
        private const string YEARLY_PREFIX = "YearlyEmailJob";

        public void PlanMountlyPaymentNotificationMail(string emailAddress, string userId, DateTime nextSubDate, string sum, string subName)
        {
            var utcNow = DateTime.UtcNow.AddDays(-14);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            var sendRecurrentEmail = new RecurrentPaymentEmailModel()
            {
                Email = emailAddress,
                NextSubDate = nextSubDate,
                Sum = sum,
                SubName = subName,
            };
            
            RecurringJob.AddOrUpdate<RobokassaMailService>($"{MOUNTHLY_PREFIX}_{userId}", x => x.SendRecurrentPaymentEmail(sendRecurrentEmail), Cron.Monthly(day, hours, minutes));
        }

        public void CancelMounthlyJob(string jobId)
        {
            RecurringJob.RemoveIfExists($"{MOUNTHLY_PREFIX}_{jobId}");
        }

        public void CancelYearlyJob(string jobId)
        {
            RecurringJob.RemoveIfExists($"{YEARLY_PREFIX}_{jobId}");
        }
    }
}
