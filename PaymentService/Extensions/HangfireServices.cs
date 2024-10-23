using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;

namespace PaymentService.Extensions;

/// <summary>
/// Статический класс для регистрации сервисов Hangfire
/// </summary>
public static class HangfireServices
{
    /// <summary>
    /// Добавляет сервисы Hangfire в коллекцию служб
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Получаем строку подключения из конфигурации
        var connString = configuration.GetSection("DbSettings:ConnectionString").Value;

        // Создаем объект MongoUrlBuilder с использованием строки подключения
        var mongoUrlBuilder = new MongoUrlBuilder(connString);

        // Создаем клиент MongoDB
        var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

        // Добавляем сервисы Hangfire в коллекцию служб
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

        // Добавляем сервер Hangfire в коллекцию служб
        services.AddHangfireServer(serverOptions =>
        {
            // Устанавливаем очереди для сервера Hangfire
            serverOptions.Queues = new[] { "emails", "chargings", "usages" };
            
            // Устанавливаем имя сервера Hangfire
            serverOptions.ServerName = "Hangfire.Mongo server 1";
        });
    }
}
