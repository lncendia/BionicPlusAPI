using AuthService.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AuthService.Models.ClientConfiguration;
using ClientConfig = AuthService.Models.ClientConfiguration.ClientConfig;

namespace AuthService.Controllers;

/// <summary>
/// Контроллер для управления конфигурацией клиента
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ConfigController : Controller
{
    /// <summary>
    /// Приватное поле для хранения конфигурации клиента
    /// </summary>
    private readonly ClientConfig _clientConfig;

    /// <summary>
    /// Конструктор контроллера конфигурации
    /// </summary>
    /// <param name="endpointConfig">Конфигурация конечных точек</param>
    /// <param name="settingsConfig">Конфигурация настроек</param>
    /// <param name="paymentConfig">Конфигурация платежей</param>
    /// <param name="reverseProxyConfig">Конфигурация обратного прокси</param>
    public ConfigController(IOptions<EndpointsConfig> endpointConfig, IOptions<SettingsConfig> settingsConfig,
        IOptions<PaymentConfig> paymentConfig, IOptions<ReverseProxyConfig> reverseProxyConfig)
    {
        // Инициализируем конфигурацию клиента
        _clientConfig = new ClientConfig
        {
            Endpoints = new ClientEndpointsConfig(endpointConfig.Value, reverseProxyConfig.Value),
            Settings = new ClientSettingsConfig(settingsConfig.Value),
            Payment = paymentConfig.Value
        };
    }

    /// <summary>
    /// Получает конфигурацию клиента
    /// </summary>
    /// <returns>Конфигурация клиента</returns>
    [HttpGet("", Name = "GetConfig")]
    public ActionResult<ClientConfig> GetConfig() => Ok(_clientConfig);
}
