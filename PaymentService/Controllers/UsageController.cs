using DomainObjects.Subscription;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Services.Interfaces;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsageController : Controller
    {
        private readonly IUsageService _usageService;

        public UsageController(IUsageService usageService)
        {
            _usageService = usageService;
        }

        [HttpGet("{userId}", Name = "Usage")]
        public async Task<ActionResult<Usage>> GetUsage(string userId)
        {
            var usage = await _usageService.GetUsageAsync(userId);
            return Ok(usage);
        }

        [HttpPost("decrement", Name = "DecrementUsage")]
        public async Task<ActionResult<Usage>> DecrementUsage([FromBody] DecrementUsage decrementUsage)
        {
            var usage = await _usageService.DecrementUsageAsync(decrementUsage.LimitKind, decrementUsage.UserId);
            return Ok(usage);
        }
    }
}
