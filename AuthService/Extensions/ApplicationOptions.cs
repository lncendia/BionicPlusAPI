using AuthService.Infrastructure;

namespace AuthService.Extensions;

public static class ApplicationOptions
{
    public static void AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .ConfigureDbSettings(configuration.GetSection("DbSettings"))
            .ConfigureJwtToken(configuration.GetSection("JwtConfig"))
            .ConfigureSmtpServer(configuration.GetSection("EmailConfiguration"))
            .ConfigureEndpoints(configuration.GetSection("EndpointsConfig"))
            .ConfigureSettingsConfig(configuration.GetSection("SettingsConfig"))
            .ConfigurePaymentConfig(configuration.GetSection("PaymentConfig"))
            .ConfigureReverseProxyConfig(configuration.GetSection("ReverseProxyConfig"));
    }
}