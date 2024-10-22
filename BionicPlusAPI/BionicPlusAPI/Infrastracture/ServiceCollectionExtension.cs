using BionicPlusAPI.Models;
using BionicPlusAPI.Models.Settings;
using Microsoft.Extensions.DependencyInjection;
using PregnancyDBMongoAccessor.Infrastracture;

namespace BionicPlusAPI.Infrastracture
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDbSettings(this IServiceCollection services, IConfigurationSection dbSettingsSection)
        {
            services.Configure<DbSettings>(dbSettingsSection);
            return services;
        }

        public static IServiceCollection ConfigureEndpoints(this IServiceCollection services, IConfigurationSection endpointsSection)
        {
            services.Configure<EndpointsConfig>(endpointsSection);
            return services;
        }

        public static IServiceCollection ConfigureS3Settings(this IServiceCollection services, IConfigurationSection dbSettingsSection)
        {
            services.Configure<S3Settings>(dbSettingsSection);
            return services;
        }
    }
}
