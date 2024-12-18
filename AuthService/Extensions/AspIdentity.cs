using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AuthService.Services.Implementations;
using IdentityLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AuthService.Extensions;

/// <summary>
/// Статический класс, представляющий методы для добавления ASP.NET Identity.
/// </summary>
public static class AspIdentity
{
    /// <summary>
    /// Добавляет ASP.NET Identity в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    ///<param name="configuration">Конфигурация приложения.</param>
    public static void AddAspIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // Создание конфигурации для MongoDB Identity
        var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
        {
            // Настройки подключения к MongoDB
            MongoDbSettings = new MongoDbSettings
            {
                // Получение строки подключения из конфигурации
                ConnectionString = configuration.GetSection("DbSettings").GetValue<string>("ConnectionString"),

                // Получение имени базы данных из конфигурации
                DatabaseName = configuration.GetSection("DbSettings").GetValue<string>("Name")
            },

            // Настройки опций Identity
            IdentityOptionsAction = opt =>
            {
                // Не требовать цифры в пароле
                opt.Password.RequireDigit = false;

                // Минимальная длина пароля
                opt.Password.RequiredLength = 8;

                // Не требовать неалфавитно-цифровых символов в пароле
                opt.Password.RequireNonAlphanumeric = false;

                // Не требовать строчных букв в пароле
                opt.Password.RequireLowercase = false;

                // Время блокировки по умолчанию
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                // Максимальное количество неудачных попыток входа перед блокировкой
                opt.Lockout.MaxFailedAccessAttempts = 5;

                // Провайдер токенов для подтверждения email
                opt.Tokens.EmailConfirmationTokenProvider = DigitTokenProvider.FOUR_DIGIT_EMAIL;

                // Провайдер токенов для сброса пароля
                opt.Tokens.PasswordResetTokenProvider = DigitTokenProvider.FOUR_DIGIT_EMAIL;

                // Требовать подтвержденный email для входа
                opt.SignIn.RequireConfirmedEmail = true;

                // Требовать уникальный email для пользователей
                opt.User.RequireUniqueEmail = true;

                // Тип утверждения для идентификатора пользователя
                opt.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;

                // Тип утверждения для имени пользователя
                opt.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Name;

                // Тип утверждения для адреса электронной почты пользователя
                opt.ClaimsIdentity.EmailClaimType = JwtRegisteredClaimNames.Email;

                // Тип утверждения для роли пользователя
                opt.ClaimsIdentity.RoleClaimType = "role";

                // Тип утверждения для штампа безопасности
                opt.ClaimsIdentity.SecurityStampClaimType = "stmp";
            }
        };

        // Настройка сервисов для MongoDB Identity
        services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
            
            // Добавление UserManager
            .AddUserManager<UserManager<ApplicationUser>>()
            
            // Добавление RoleManager
            .AddRoleManager<RoleManager<ApplicationRole>>()
            
            // Добавление стандартных провайдеров токенов
            .AddDefaultTokenProviders()
            
            // Добавление провайдера токенов для email
            .AddTokenProvider<DigitTokenProvider>(DigitTokenProvider.FOUR_DIGIT_EMAIL);
    }
}