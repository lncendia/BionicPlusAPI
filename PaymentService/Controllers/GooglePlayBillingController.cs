using DomainObjects.Subscription;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Services.Interfaces;
using PaymentService.Models.GooglePlayBilling;

namespace PaymentService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GooglePlayBillingController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<GooglePlayBillingController> _logger;

    public GooglePlayBillingController(IUserService userService, ILogger<GooglePlayBillingController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("handle", Name = "Handle webhook")]
    public async Task<ActionResult<string>> Handle(GoogleWebhook webhook)
    {
        try
        {
            var sign = _robokassaService.VerifySignature(SignatureValue, OutSum, InvId, Shp_userId, Shp_isFirst, Shp_subscriptionId);

            if (!sign) return BadRequest();

            var isFirst = bool.Parse(Shp_isFirst);

            var subscription = await _subscriptionService.GetSubscription(subscriptionId);

            if(subscription.Status == SubscriptionStatus.ACTIVE)
            {
                return true;
            }

            var user = await _userManager.FindByIdAsync(userId);

            var userEmail = user.Email;

            var planId = subscription.PlanId;

            var plan = await _planService.GetPlan(planId);
                
            if (plan == null)
            {
                throw new ArgumentException($"Plan with id {planId} not found");
            }
                
            await _paymentProcessorService.ProcessSuccessPayment(Shp_userId, InvId, isFirst, Shp_subscriptionId);
                
            await _subscriptionService.ActivateSubscription(subscriptionId);

            await _subscriptionService.SetSubscription(userId, planId, subscriptionId);
                
            return Ok($"OK{InvId}");
        }
        catch(Exception ex)
        {
            _logger.LogError($"error occured on success ==================> {ex.Message}");
            throw;
        }
    }
}