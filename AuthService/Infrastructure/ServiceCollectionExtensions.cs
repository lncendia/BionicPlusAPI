
using AuthService.Models;
using MailSenderLibrary.Models;

namespace AuthService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDbSettings(this IServiceCollection services, IConfigurationSection dbSettingsSection)
        {
            services.Configure<DbSettings>(dbSettingsSection);
            return services;
        }

        public static IServiceCollection ConfigureJwtToken(this IServiceCollection services, IConfigurationSection jwtSection)
        {
            services.Configure<JwtConfig>(jwtSection);
            return services;
        }

        public static IServiceCollection ConfigureSmtpServer(this IServiceCollection services, IConfigurationSection smtpSection)
        {
            services.Configure<EmailConfiguration>(smtpSection);
            return services;
        }

        public static IServiceCollection ConfigureEndpoints(this IServiceCollection services, IConfigurationSection endpointSettingsSection)
        {
            services.Configure<EndpointsConfig>(endpointSettingsSection);
            return services;
        }

        public static IServiceCollection ConfigureSettingsConfig(this IServiceCollection services, IConfigurationSection settingsSection)
        {
            services.Configure<SettingsConfig>(settingsSection);
            return services;
        }

        public static IServiceCollection ConfigurePaymentConfig(this IServiceCollection services, IConfigurationSection paymentSection)
        {
            services.Configure<PaymentConfig>(paymentSection);
            return services;
        }
    }
}
