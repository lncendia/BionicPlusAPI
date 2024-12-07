using Hangfire;
using PaymentService.Models.Emails;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.Robokassa.Implementations.Recurring
{
    public class MailRecurringService 
    {
        private const string MonthlyPrefix = "MounthlyEmailJob";
        private const string YearlyPrefix = "YearlyEmailJob";

        public void PlanMonthlyPaymentNotificationMail(RecurrentPaymentEmailModel emailModel)
        {
            var utcNow = DateTime.UtcNow.AddDays(-14);

            var day = utcNow.Day;
            var hours = utcNow.Hour;
            var minutes = utcNow.Minute;
            
            RecurringJob.AddOrUpdate<IPaymentMailService>($"{MonthlyPrefix}_{emailModel.UserId}", x => x.SendRecurrentPaymentEmail(emailModel), Cron.Monthly(day, hours, minutes));
        }

        public void CancelMounthlyJob(string jobId)
        {
            RecurringJob.RemoveIfExists($"{MonthlyPrefix}_{jobId}");
        }

        public void CancelYearlyJob(string jobId)
        {
            RecurringJob.RemoveIfExists($"{YearlyPrefix}_{jobId}");
        }
    }
}
