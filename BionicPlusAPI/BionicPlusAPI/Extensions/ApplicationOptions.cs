namespace BionicPlusAPI.Extensions;

/// <summary>
/// Статический класс для добавления настроек приложения в коллекцию служб
/// </summary>
public static class ApplicationOptions
{
    /// <summary>
    /// Добавляет настройки приложения в коллекцию служб
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services

            // Настраиваем настройки базы данных
            .ConfigureDbSettings(configuration.GetSection("DbSettings"))

            // Настраиваем настройки S3
            .ConfigureS3Settings(configuration.GetSection("S3Settings"))

            // Настраиваем конечные точки
            .ConfigureEndpoints(configuration.GetSection("EndpointsConfig"));
    }
}