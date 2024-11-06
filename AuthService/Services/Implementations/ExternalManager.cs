using AuthService.Configuration;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AuthService.Services.Implementations;

/// <summary>
/// ExternalManager class that manages external processors.
/// </summary>
public class ExternalManager : IExternalManager
{
    /// <summary>
    /// Dictionary to store external processors.
    /// </summary>
    private readonly Dictionary<string, IExternalProcessor> _externalProcessors;

    /// <summary>
    /// Constructor for ExternalManager.
    /// </summary>
    /// <param name="options">Options for the external manager.</param>
    /// <param name="httpClient">HTTP client for making requests.</param>
    public ExternalManager(IOptions<SettingsConfig> options, HttpClient httpClient)
    {
        // Initialize the dictionary with external processors
        _externalProcessors = new Dictionary<string, IExternalProcessor>
        {
            {
                "Google",
                new ExternalOidcManager("Google", options.Value.ExternalGoogleAudiences.Split(','), "https://accounts.google.com", httpClient)
            }
        };
        
        // todo: Здесь можно регистрировать провайдеры внешней аутентификации.
        // Для провайдеров OIDC по типу Google или Яндекс можно использовать ExternalOidcManager.
        // Для других типов, например OAuth без OIDC, можно написать другую реализацию и расширить интерфейс.
    }

    /// <inheritdoc/>
    /// <summary>
    /// Gets the external processor by provider name.
    /// </summary>
    public IExternalProcessor GetProcessor(string providerName) => _externalProcessors[providerName];
}
