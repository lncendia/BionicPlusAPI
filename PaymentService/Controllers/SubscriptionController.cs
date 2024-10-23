using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using PaymentService.Services.Interfaces;
using System.Security.Claims;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost(Name = "SetFreeSubscription")]
        public async Task<ActionResult<string>> SetFreeSubscription([FromQuery, Required] string userId)
        {
            var subscriptionId = await _subscriptionService.SetFreeSubscription(userId, true);
            return Ok(subscriptionId);
        }

        [HttpGet(Name = "GetSubscription")]
        public async Task<IActionResult> GetSubscription([FromQuery, Required] string id)
        {
            var subscription = await _subscriptionService.GetSubscription(id);
            return Ok(subscription);
        }

        [HttpGet("promocode", Name = "GetPromocode")]
        public async Task<IActionResult> GetPromocode([FromQuery, Required] string promocode)
        {
            var promocodeModel = await _subscriptionService.GetPromocode(promocode);
            return Ok(promocodeModel);
        }

        [HttpGet("price/calculate", Name = "CalculatePrice")]
        public async Task<IActionResult> CalculatePrice([FromQuery, Required] string promocode, [FromQuery, Required] string planId)
        {
            var promocodeModel = await _subscriptionService.CalculatePrice(planId, promocode);
            return Ok(promocodeModel);
        }

        [HttpPost("cancel", Name = "CancelUserSubscription")]
        public async Task<IActionResult> CancelUserSubscription()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subscriptionId = await _subscriptionService.SetFreeSubscription(userId, true, false);
            return Ok();
        }
    }
}
