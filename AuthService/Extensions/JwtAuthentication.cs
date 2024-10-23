using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Extensions;

/// <summary>
/// Класс для настройки аутентификации JWT в приложении.
/// </summary>
public static class JwtAuthentication
{
    /// <summary>
    /// Добавляет настройки аутентификации JWT в коллекцию служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Получение конфигурационных данных из файла конфигурации
        var jwtConfigSection = configuration.GetSection("JwtConfig");

        // Получение значения ValidIssuer из конфигурации
        var validIssuer = jwtConfigSection.GetValue<string>("ValidIssuer");

        // Получение значения ValidAudiences из конфигурации и разделение на массив строк
        var validAudiences = jwtConfigSection.GetValue<string>("ValidAudiences").Split(",");

        // Получение значения IssuerSigningKey из конфигурации
        var issuerSigningKey = jwtConfigSection.GetValue<string>("IssuerSigningKey");

        // Настройка параметров валидации токена
        var tokenValidationParameters = new TokenValidationParameters
        {
            // Проверка подписи токена
            ValidateIssuerSigningKey = true,

            // Проверка издателя токена
            ValidateIssuer = true,

            // Проверка аудитории токена
            ValidateAudience = true,

            // Проверка срока действия токена
            ValidateLifetime = true,

            // Издатель токена
            ValidIssuer = validIssuer,

            // Аудитории токена
            ValidAudiences = validAudiences,

            // Ключ для подписи токена
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey)),

            // Отключение временного сдвига
            ClockSkew = TimeSpan.Zero
        };

        // Настройка аутентификации
        services
            .AddAuthentication(x =>
            {
                // Схема аутентификации по умолчанию
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                // Схема вызова аутентификации по умолчанию
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(t =>
            {
                // Не требовать HTTPS для метаданных
                t.RequireHttpsMetadata = false;

                // Сохранять токен в контексте запроса
                t.SaveToken = true;

                // Параметры валидации токена
                t.TokenValidationParameters = tokenValidationParameters;
            });
    }
}