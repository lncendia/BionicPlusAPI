using Hangfire;
using HangfireBasicAuthenticationFilter;

namespace PaymentService.Extensions;

public static class HangfireDashboardExtensions
{
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

        // Инициализируем дашборд
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
            DisplayStorageConnectionString = false,
        });
    }
}