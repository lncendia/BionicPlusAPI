using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using PaymentService.Services.Interfaces;
using System.Security.Claims;
using DomainObjects.Pregnancy.Localizations;
using DomainObjects.Subscription;
using PaymentService.Constants;

namespace PaymentService.Controllers;

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
    public async Task<IActionResult> CalculatePrice([FromQuery, Required] string promocode,
        [FromQuery, Required] string planId)
    {
        var promocodeModel = await _subscriptionService.CalculatePrice(planId, promocode);
        return Ok(promocodeModel);
    }

    [HttpPost("cancel", Name = "CancelUserSubscription")]
    public async Task<IActionResult> CancelUserSubscription()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await CancelSubscription(userId);

        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("cancel", Name = "CancelUserSubscription")]
    public async Task<IActionResult> CancelUserSubscription([FromQuery, Required] string userId,
        [FromQuery, Required] string hash,
        [FromQuery] LocalizationsLanguage language = LocalizationsLanguage.en)
    {
        // Get user by id
        var user = await _userService.GetUserById(userId);

        // Verification of the users existence
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Verification user by hash
        var hashVerificationResult = _userService.VerifyUserIdHash(userId, hash);

        // Return unauthorized result
        if (!hashVerificationResult)
        {
            return Unauthorized("Invalid hash.");
        }

        // Subscription cancellation logic
        await CancelSubscription(userId);

        var result = language switch
        {
            LocalizationsLanguage.en => SubscriptionCancellationAnswers.En,
            LocalizationsLanguage.ru => SubscriptionCancellationAnswers.Ru,
            _ => SubscriptionCancellationAnswers.En
        };

        return Ok(result);
    }

    [HttpPost("googleSubscription", Name = "SetGooglePurchaseToken")]
    public async Task<IActionResult> CancelSubscription([FromQuery, Required] string subscriptionId,
        [FromQuery, Required] string googleOrderId,
        [FromQuery, Required] string googlePurchaseToken)
    {
        if (string.IsNullOrWhiteSpace(googleOrderId) || string.IsNullOrWhiteSpace(googlePurchaseToken))
        {
            return BadRequest("Invalid google order id or google purchase token.");
        }
        
        await _subscriptionService.SetGooglePurchaseToken(subscriptionId, googleOrderId, googlePurchaseToken);

        return Ok();
    }

    [HttpGet("googleSubscription", Name = "GetByOrderId")]
    public async Task<ActionResult<Subscription>> GetSubscriptionByOrderId([FromQuery, Required] string orderId)
    {
        return await _subscriptionService.GetSubscriptionByGoogleOrderId(orderId);
    }

    /// <summary>
    /// Method for cancellation user subscription
    /// </summary>
    /// <param name="userId">User id</param>
    [NonAction]
    private async Task CancelSubscription(string userId)
    {
        var subscriptionId = await _userService.GetActiveSubscription(userId);
        if (subscriptionId == null) return;
        await _subscriptionService.DeactivateSubscription(subscriptionId);
        _subscriptionService.CancelPaymentReccuringJobs(userId); //todo: выноситься в пеймент процессор
    }
}