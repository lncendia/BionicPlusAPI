using Amazon.Runtime;
using AuthService.Configuration;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AuthService.Models.ClientConfiguration;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConfigController : Controller
{
    private readonly ClientEndpointsConfig _endpointConfig;
    private readonly ClientSettingsConfig _settingsConfig;
    private readonly PaymentConfig _paymentConfig;

    public ConfigController(IOptions<EndpointsConfig> endpointConfig, IOptions<SettingsConfig> settingsConfig,
        IOptions<PaymentConfig> paymentConfig, IOptions<ReverseProxyConfig> reverseProxyConfig)
    { ;
        _endpointConfig = new ClientEndpointsConfig(endpointConfig.Value, reverseProxyConfig.Value);
        _settingsConfig = new ClientSettingsConfig(settingsConfig.Value);
        _paymentConfig = paymentConfig.Value;
    }

    [HttpGet("", Name = "GetConfig")]
    public ActionResult<ClientConfig> GetConfig()
    {
        return Ok(new ClientConfig
        {
            Endpoints = _endpointConfig,
            Settings = _settingsConfig,
            Payment = _paymentConfig
        });
    }
}