using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;

namespace PaymentService.Extensions;

/// <summary>
/// A static class for registering Hangfire services
/// </summary>
public static class HangfireServices
{
    /// <summary>
    /// Adds Hangfire services to the collection of services
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Получаем строку подключения из конфигурации

        // Создаем объект MongoUrlBuilder с использованием строки подключения
        // Get database connection string

        // Adding Hangfire services to the collection of services
        services.AddHangfire(hangfireCfg => hangfireCfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMongoStorage(mongoClient, "HangFireDb", new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                },
                Prefix = "hangfire.mongo",
                CheckConnection = true
            })
        );

        // Adding the Hangfire server to the collection of services
        services.AddHangfireServer(serverOptions =>
        {
            // Setting up queues for the Hangfire server
            serverOptions.Queues = new[] { "emails", "chargings", "usages", "expired_subscriptions" };
            
            // Set Hangfire server name
            serverOptions.ServerName = "Hangfire.Mongo server 1";
        });
    }
    
    /// <summary>
    /// Schedules a recurring job to cancel expired subscriptions on a minutely basis.
    /// </summary>
    /// <param name="app">Application builder</param>
    public static void RunCancellationSubscriptionsJob(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        recurringJobManager.AddOrUpdate<SubscriptionService>(
            "CancellationSubscriptionsJob",
            x => x.CancelExpiredSubscriptions(),
            Cron.Minutely);
    }
}