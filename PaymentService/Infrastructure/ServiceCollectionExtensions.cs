using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MailSenderLibrary.Models;
using MongoDB.Driver;
using PaymentService.Models;
using PaymentService.Models.Robokassa;
using SubscriptionDBMongoAccessor.Infrastracture;

namespace PaymentService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDbSettings(this IServiceCollection services, IConfigurationSection dbSettingsSection)
        {
            services.Configure<DbSettings>(dbSettingsSection);
            return services;
        }

        public static IServiceCollection ConfigurePlansSettings(this IServiceCollection services, IConfigurationSection plansSettingsSection)
        {
            services.Configure<PlansConfiguration>(plansSettingsSection);
            return services;
        }

        public static IServiceCollection ConfigureMerchantInfoSettings(this IServiceCollection services, IConfigurationSection merchantSettingsConfiguration)
        {
            services.Configure<MerchantInfo>(merchantSettingsConfiguration);
            return services;
        }

        public static IServiceCollection ConfigureAllowedIps(this IServiceCollection services, IConfigurationSection allowedIpsSectiion)
        {
            services.Configure<AllowedIps>(allowedIpsSectiion);
            return services;
        }

        public static IServiceCollection ConfigureSmtpServer(this IServiceCollection services, IConfigurationSection smtpSection)
        {
            services.Configure<EmailConfiguration>(smtpSection);
            return services;
        }

        public static IServiceCollection ConfigureEndpoints(this IServiceCollection services, IConfigurationSection endpointsSection)
        {
            services.Configure<EndpointsConfig>(endpointsSection);
            return services;
        }

        public static IServiceCollection ConfigureHangFire(this IServiceCollection services, string connectionString)
        {
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            services.AddHangfire(configuration => configuration
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


            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.Queues = new string[] { "emails", "chargings", "usages" };
                serverOptions.ServerName = "Hangfire.Mongo server 1";
            });
            return services;
        }
    }
}
