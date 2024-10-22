using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Xml.Linq;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : Controller
    {
        private readonly EndpointsConfig _endpointConfig;
        private readonly SettingsConfig _settingsConfig;
        private readonly PaymentConfig _paymentConfig;

        public ConfigController(IOptions<EndpointsConfig> endpointConfig, IOptions<SettingsConfig> settingsConfig, IOptions<PaymentConfig> paymentConfig)
        {
            _endpointConfig = endpointConfig.Value;
            _settingsConfig = settingsConfig.Value;
            _paymentConfig = paymentConfig.Value;
        }

        [HttpGet("", Name = "GetConfig")]
        public ActionResult<Config> GetConfig()
        {
            return Ok(new Config { Endpoints = _endpointConfig, Settings = _settingsConfig, Payment = _paymentConfig });
        }
    }
}
