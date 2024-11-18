using Hangfire;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Implementations;

public class UsageRecurringService
{
    private const string MonthlyPrefix = "MounthlyUsageJob";
    private const string YearlyPrefix = "YearlyUsageJob";
        
    public void PlanMonthlyRefill(string userId, string planId)
    {
        var utcNow = DateTime.UtcNow;

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<ISubscriptionService>($"{MonthlyPrefix}_{userId}", x => x.InsureSubscription(userId), Cron.Monthly(day, hours, minutes));
    }

    public void PlanYearlyRefill(string userId, string planId)
    {
        var utcNow = DateTime.UtcNow;

        var day = utcNow.Day;
        var hours = utcNow.Hour;
        var minutes = utcNow.Minute;

        RecurringJob.AddOrUpdate<ISubscriptionService>($"{YearlyPrefix}_{userId}", x => x.InsureSubscription(userId), Cron.Yearly(day, hours, minutes));
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