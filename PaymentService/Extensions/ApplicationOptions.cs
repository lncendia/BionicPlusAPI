namespace PaymentService.Extensions;

/// <summary>
/// Static class for adding application settings to a collection of services
/// </summary>
public static class ApplicationOptions
{
    /// <summary>
    /// Adds application settings to the services collection
    /// </summary>
    /// <param name="services">Collection of servers</param>
    /// <param name="configuration">Application configuration</param>
    public static void AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services

            // Settings up database settings
            .ConfigureDbSettings(configuration.GetSection("DbSettings"))

            // Settings up plans configuration
            .ConfigurePlansSettings(configuration.GetSection("PlansConfiguration"))

            // Settings up jwt configuration
            .ConfigureEncryptionSettings(configuration.GetSection("EncryptionConfig"))

            // settings up merchant info
            .ConfigureMerchantInfoSettings(configuration.GetSection("MerchantInfo"))

            // Settings up allowed IPs
            .ConfigureAllowedIps(configuration.GetSection("AllowedIps"))

            // Setting up SMTP-servers
            .ConfigureSmtpServer(configuration.GetSection("EmailConfiguration"))

            // Setting up endpoints
            .ConfigureEndpoints(configuration.GetSection("EndpointsConfig"))

            // Setting up subscriptions
            .ConfigureSubscriptionsConfig(configuration.GetSection("SubscriptionConfig"))

            // Settings up google play config
            .ConfigureGooglePlayConfig(configuration.GetSection("GooglePlayConfig"));
    }
}