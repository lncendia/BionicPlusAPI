namespace AuthService.Extensions;

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

            // Настраиваем настройки JWT-токена
            .ConfigureJwtToken(configuration.GetSection("JwtConfig"))

            // Настраиваем настройки SMTP-сервера
            .ConfigureSmtpServer(configuration.GetSection("EmailConfiguration"))

            // Настраиваем конечные точки
            .ConfigureEndpoints(configuration.GetSection("EndpointsConfig"))

            // Настраиваем настройки конфигурации
            .ConfigureSettingsConfig(configuration.GetSection("SettingsConfig"))

            // Настраиваем настройки платежей
            .ConfigurePaymentConfig(configuration.GetSection("PaymentConfig"))

            // Настраиваем настройки обратного прокси
            .ConfigureReverseProxyConfig(configuration.GetSection("ReverseProxyConfig"));
    }
}