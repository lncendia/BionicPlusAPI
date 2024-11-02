using MailSenderLibrary.Models;
using PaymentService.Models;
using PaymentService.Models.Robokassa;
using SubscriptionDBMongoAccessor.Infrastracture;
using SubscriptionsConfig = PaymentService.Models.SubscriptionsConfig;

namespace PaymentService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDbSettings(this IServiceCollection services, IConfigurationSection dbSettingsSection)
    {
        services.Configure<DbSettings>(dbSettingsSection);
        return services;
    }
    
    public static IServiceCollection ConfigureJwtSettings(this IServiceCollection services, IConfigurationSection JwtSettingsSection)
    {
        services.Configure<JwtConfig>(JwtSettingsSection);
        return services;
    }

    public static IServiceCollection ConfigurePlansSettings(this IServiceCollection services, IConfigurationSection plansSettingsSection)
    {
        services.Configure<PlansConfiguration>(plansSettingsSection);
        return services;
    }

    public static IServiceCollection ConfigureMerchantInfoSettings(this IServiceCollection services, IConfigurationSection merchantSettingsConfiguration)
    {
        services.Configure<MerchantInfo>(merchantSettingsConfiguration);
        return services;
    }

    public static IServiceCollection ConfigureAllowedIps(this IServiceCollection services, IConfigurationSection allowedIpsSectiion)
    {
        services.Configure<AllowedIps>(allowedIpsSectiion);
        return services;
    }

    public static IServiceCollection ConfigureSmtpServer(this IServiceCollection services, IConfigurationSection smtpSection)
    {
        services.Configure<EmailConfiguration>(smtpSection);
        return services;
    }

    public static IServiceCollection ConfigureEndpoints(this IServiceCollection services, IConfigurationSection endpointsSection)
    {
        services.Configure<EndpointsConfig>(endpointsSection);
        return services;
    }

    public static IServiceCollection ConfigureSubscriptionsConfig(this IServiceCollection services,
        IConfigurationSection subscriptionsSection)
    {
        services.Configure<SubscriptionsConfig>(subscriptionsSection);
        return services;
    }
}