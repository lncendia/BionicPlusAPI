using Hangfire;

namespace PaymentService.Services.Robokassa.Implementations.Recurring;

public class MailRecurringService 
{
    private const string MounthlyPrefix = "MounthlyEmailJob";
    private const string YearlyPrefix = "YearlyEmailJob";

    public void PlanMountlyPaymentNotificationMail(string emailAddress, string userId, DateTime nextSubDate, string sum, string subName)
    {
        var utcNow = DateTime.UtcNow.AddDays(-14);

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<RobokassaMailService>($"{MounthlyPrefix}_{userId}", x => x.SendRecurrentPaymentEmailAsync(emailAddress, nextSubDate, sum, subName), Cron.Monthly(day, hours, minutes));
    }

    public void PlanMinutelyMail(string emailAddress, string userId, DateTime nextSubDate, string sum, string subName)
    {
        var utcNow = DateTime.UtcNow;

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;


        RecurringJob.AddOrUpdate<RobokassaMailService>($"Minute_{userId}", x => x.SendRecurrentPaymentEmailAsync(emailAddress, nextSubDate, sum, subName), "0 */3 * ? * *");
    }

    public void PlanYearlyMail(string emailAddress, string userId, DateTime nextSubDate, string sum, string subName)
    {
        var utcNow = DateTime.UtcNow.AddDays(-14);

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<RobokassaMailService>($"{YearlyPrefix}_{userId}", x => x.SendRecurrentPaymentEmailAsync(emailAddress, nextSubDate, sum, subName), Cron.Yearly(day, hours, minutes));
    }

    public void CancelMounthlyJob(string jobId)
    {
        RecurringJob.RemoveIfExists($"{MounthlyPrefix}_{jobId}");
    }

    public void CancelYearlyJob(string jobId)
    {
        RecurringJob.RemoveIfExists($"{YearlyPrefix}_{jobId}");
    }
}