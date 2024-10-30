using Hangfire;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Robokassa.Implementations.Recurring;

public class UsageRecurringService
{
    private const string MounthlyPrefix = "MounthlyUsageJob";
    private const string YearlyPrefix = "YearlyUsageJob";

    public void PlanMountlyRefill(string userId, string planId)
    {
        var utcNow = DateTime.UtcNow;

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<ISubscriptionService>($"{MounthlyPrefix}_{userId}", x => x.InsureSubscription(userId), Cron.Monthly(day, hours, minutes));
    }

    public void PlanYearlyRefill(string userId, string planId)
    {
        var utcNow = DateTime.UtcNow;

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<ISubscriptionService>($"{YearlyPrefix}_{userId}", x => x.InsureSubscription(userId), Cron.Yearly(day, hours, minutes));
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