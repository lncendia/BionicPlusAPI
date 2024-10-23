using AuthService.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AuthService.Models.ClientConfiguration;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConfigController : Controller
{
    private readonly EndpointsConfig _endpointConfig;
    private readonly SettingsConfig _settingsConfig;
    private readonly PaymentConfig _paymentConfig;
    private readonly ReverseProxyConfig _reverseProxyConfig;

    public ConfigController(IOptions<EndpointsConfig> endpointConfig, IOptions<SettingsConfig> settingsConfig,
        IOptions<PaymentConfig> paymentConfig, IOptions<ReverseProxyConfig> reverseProxyConfig)
    {
        _reverseProxyConfig = reverseProxyConfig.Value;
        _endpointConfig = endpointConfig.Value;
        _settingsConfig = settingsConfig.Value;
        _paymentConfig = paymentConfig.Value;
    }

    [HttpGet("", Name = "GetConfig")]
    public ActionResult<ClientConfig> GetConfig()
    {
        return Ok(new ClientConfig
        {
            Endpoints = new ClientEndpointsConfig(_endpointConfig, _reverseProxyConfig),
            Settings = _settingsConfig,
            Payment = _paymentConfig
        });
    }
}