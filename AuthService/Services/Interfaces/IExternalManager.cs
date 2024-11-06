namespace AuthService.Services.Interfaces;

/// <summary>
/// Interface that manages external processors.
/// </summary>
public interface IExternalManager
{
    /// <summary>
    /// Gets the external processor by provider name.
    /// </summary>
    /// <param name="providerName">The name of the provider.</param>
    /// <returns>The external processor for the given provider name.</returns>
    IExternalProcessor GetProcessor(string providerName);
}

