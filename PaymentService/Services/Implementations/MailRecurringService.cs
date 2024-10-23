using Hangfire;
using System.Net.Mail;

namespace PaymentService.Services.Implementations
{
    public class MailRecurringService 
    {
        private const string MOUNTHLY_PREFIX = "MounthlyEmailJob";
        private const string YEARLY_PREFIX = "YearlyEmailJob";

        public void PlanMountlyPaymentNotificationMail(string emailAdress, string userId, DateTime nextSubDate, string sum, string subName)
        {
            var utcNow = DateTime.UtcNow.AddDays(-14);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<RobokassaMailService>($"{MOUNTHLY_PREFIX}_{userId}", x => x.SendRecurrentPaymentEmailAsync(emailAdress, nextSubDate, sum, subName), Cron.Monthly(day, hours, minutes));
        }

        public void PlanMinutelyMail(string emailAdress, string userId, DateTime nextSubDate, string sum, string subName)
        {
            var utcNow = DateTime.UtcNow;

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;


            RecurringJob.AddOrUpdate<RobokassaMailService>($"Minute_{userId}", x => x.SendRecurrentPaymentEmailAsync(emailAdress, nextSubDate, sum, subName), "0 */3 * ? * *");
        }

        public void PlanYearlyMail(string emailAdress, string userId, DateTime nextSubDate, string sum, string subName)
        {
            var utcNow = DateTime.UtcNow.AddDays(-14);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;

            RecurringJob.AddOrUpdate<RobokassaMailService>($"{YEARLY_PREFIX}_{userId}", x => x.SendRecurrentPaymentEmailAsync(emailAdress, nextSubDate, sum, subName), Cron.Yearly(day, hours, minutes));
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
