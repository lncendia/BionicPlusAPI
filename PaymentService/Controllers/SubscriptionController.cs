﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserService _userService;
        
        public SubscriptionController(ISubscriptionService subscriptionService, IUserService userService)
        {
            _subscriptionService = subscriptionService;
            _userService = userService;
        }

        [HttpPost(Name = "CreateFreeSubscription")]
        public async Task<ActionResult<string>> SetFreeSubscription([FromQuery, Required] string userId)
        {
            var subscriptionId = await _subscriptionService.CreateFreeSubscription(userId, true);
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
            var subscriptionId = await _subscriptionService.CreateFreeSubscription(userId, true, false);
            var result = await _userService.SetSubscription(userId, subscriptionId, isFreePlan: true);
            
            return Ok(result);
        }
    }
}
