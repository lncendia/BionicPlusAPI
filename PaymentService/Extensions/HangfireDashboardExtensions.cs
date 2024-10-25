using Hangfire;
using HangfireBasicAuthenticationFilter;

namespace PaymentService.Extensions;

/// <summary>
/// Расширение для настройки Hangfire Dashboard.
/// </summary>
public static class HangfireDashboardExtensions
{
    /// <summary>
    /// Создает и настраивает Hangfire Dashboard.
    /// </summary>
    /// <param name="app">Экземпляр IApplicationBuilder.</param>
    /// <param name="configuration">Экземпляр IConfiguration.</param>
    public static void CreateHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
    {
        // Получение конфигурационных данных из файла конфигурации
        var reverseProxyString = configuration.GetValue<string>("ReverseProxyConfig:PaymentProxy");

        // Инициализируем переменную prefix значением null
        string? prefix = null;

        // Проверяем, не является ли строка reverseProxyString пустой или null
        if (!string.IsNullOrEmpty(reverseProxyString))
        {
            // Создаем объект Uri из строки reverseProxyString
            var reverseProxyUri = new Uri(reverseProxyString);

            // Присваиваем переменной prefix значение LocalPath из объекта Uri
            prefix = reverseProxyUri.LocalPath;
        }

        // Настраиваем Hangfire Dashboard с использованием полученного префикса
        app.UseHangfireDashboard("/hangfire/dashboard", new DashboardOptions
        {
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = "admin",
                    Pass = "admin"
                },
            },
            PrefixPath = prefix,
            DisplayStorageConnectionString = false
        });
    }
}
