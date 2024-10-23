namespace PaymentService.Extensions;

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

            // Настраиваем настройки планов
            .ConfigurePlansSettings(configuration.GetSection("PlansConfiguration"))

            // Настраиваем настройки информации о продавце
            .ConfigureMerchantInfoSettings(configuration.GetSection("MerchantInfo"))

            // Настраиваем разрешенные IP-адреса
            .ConfigureAllowedIps(configuration.GetSection("AllowedIps"))

            // Настраиваем настройки SMTP-сервера
            .ConfigureSmtpServer(configuration.GetSection("EmailConfiguration"))

            // Настраиваем конечные точки
            .ConfigureEndpoints(configuration.GetSection("EndpointsConfig"));
    }
}