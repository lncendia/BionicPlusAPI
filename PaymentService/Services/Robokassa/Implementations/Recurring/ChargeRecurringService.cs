using Hangfire;

namespace PaymentService.Services.Robokassa.Implementations.Recurring;

public class ChargeRecurringService
{
    private const string MounthlyPrefix = "MounthlyChargeJob";
    private const string YearlyPrefix = "YearlyChargeJob";

    public void PlanMountlyCharge(string userId, string planId, string firstInvoiceId)
    {
        var utcNow = DateTime.UtcNow.AddHours(-2);

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<RobokassaClient>($"{MounthlyPrefix}_{userId}", x => x.ChargeRecurrentPayment(firstInvoiceId, planId, userId), Cron.Monthly(day, hours, minutes));
    }

    public void PlanYearlyCharge(string userId, string planId, string firstInvoiceId)
    {
        var utcNow = DateTime.UtcNow.AddHours(-2);

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<RobokassaClient>($"{YearlyPrefix}_{userId}", x => x.ChargeRecurrentPayment(firstInvoiceId, planId, userId), Cron.Yearly(day, hours, minutes));
    }

    public bool FindJobById(string userId, bool isMountly)
    {
        using var connection = JobStorage.Current.GetConnection();
        var job = connection.GetJobData(isMountly ? $"{MounthlyPrefix}_{userId}" : $"{YearlyPrefix}_{userId}");
        return job != null;
    }

    public void CancelMounthlyJob(string userId)
    {
        RecurringJob.RemoveIfExists($"{MounthlyPrefix}_{userId}");
    }

    public void CancelYearlyJob(string userId)
    {
        RecurringJob.RemoveIfExists($"{YearlyPrefix}_{userId}");
    }
}