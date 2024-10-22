using Microsoft.AspNetCore.Mvc;
using PaymentService.Services.Interfaces;
using DomainObjects.Subscription;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet("", Name = "GetPlan")]
        public async Task<ActionResult<Plan>> GetPlan([FromQuery] string planId)
        {
            var plan = await _planService.GetPlan(planId);
            return Ok(plan);
        }
    }
}
