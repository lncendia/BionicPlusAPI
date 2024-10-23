using Microsoft.OpenApi.Models;

namespace BionicPlusAPI.Extensions;

/// <summary>
/// Класс для настройки Swagger в приложении.
/// </summary>
public static class SwaggerServices
{
    /// <summary>
    /// Добавляет настройки Swagger в коллекцию служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddSwaggerServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Получение конфигурационных данных из файла конфигурации
        var reverseProxyString = configuration.GetValue<string>("ReverseProxyConfig:ApiProxy");

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

        // Добавляем Swagger генератор в коллекцию служб
        services.AddSwaggerGen(options =>
        {
            // Добавляем префикс всем эндпоинтам для корректной работы через обратный прокси
            if (prefix is not null) options.DocumentFilter<PathPrefixInsertDocumentFilter>(prefix);

            // Поддержка необязательных ссылочных типов
            options.SupportNonNullableReferenceTypes();

            // Добавляем определение безопасности для Bearer токена
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                // Тип схемы безопасности
                Type = SecuritySchemeType.Http,

                // Формат Bearer токена
                BearerFormat = "JWT",

                // Местоположение параметра
                In = ParameterLocation.Header,

                // Схема
                Scheme = "bearer",

                // Описание
                Description = "Please insert JWT token into field"
            });

            // Добавляем требование безопасности для Bearer токена
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    // Определение схемы безопасности
                    new OpenApiSecurityScheme
                    {
                        // Ссылка на схему безопасности
                        Reference = new OpenApiReference
                        {
                            // Тип ссылки
                            Type = ReferenceType.SecurityScheme,

                            // Идентификатор схемы безопасности
                            Id = "Bearer"
                        }
                    },
                    // Пустой массив для указания, что схема безопасности применяется ко всем операциям
                    Array.Empty<string>()
                }
            });
        });
    }
}