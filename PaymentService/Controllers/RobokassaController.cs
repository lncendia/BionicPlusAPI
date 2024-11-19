using DomainObjects.Pregnancy.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Services.Interfaces;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DomainObjects.Subscription;
using PaymentService.Models.GooglePlayBilling;
using PaymentService.Models.Robokassa;

namespace PaymentService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RobokassaController : Controller
{
    private readonly IUserService _userService;
    private readonly IPaymentProcessor<RobokassaCallback> _robokassaPaymentProcessor;
    private readonly IPaymentProcessor<GoogleCallback> _googlePlayBillingProcessor;
    private readonly ILogger<RobokassaController> _logger;

    public RobokassaController(IUserService userService, ILogger<RobokassaController> logger,
        IPaymentProcessor<RobokassaCallback> paymentProcessorService,
        IPaymentProcessor<GoogleCallback> googlePlayBillingProcessor)
    {
        _userService = userService;
        _logger = logger;
        _robokassaPaymentProcessor = paymentProcessorService;
        _googlePlayBillingProcessor = googlePlayBillingProcessor;
    }

    [Authorize]
    [HttpPost("checkout", Name = "Generate checkout link")]
    public async Task<IActionResult> Checkout([FromBody] UserAgreement userAgreement, [FromQuery] string planId,
        [FromQuery] string? promocode, [FromQuery] PaymentServiceType service)
    {
        if (!userAgreement.PersonalDataAgreement || !userAgreement.RecurringPaymentAgreement ||
            !userAgreement.UserAgreementAgreement)
        {
            return BadRequest("You must allow all user agreements.");
        }

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _userService.SaveUserAgreements(userAgreement, userId);

        var result = service switch
        {
            PaymentServiceType.Robokassa => _robokassaPaymentProcessor.CheckoutAsync(planId, promocode),
            PaymentServiceType.GooglePlay => _googlePlayBillingProcessor.CheckoutAsync(planId, promocode),
            _ => throw new ArgumentOutOfRangeException(nameof(service), service, null)
        };

        return Ok(await result);
    }

    [HttpPost("result", Name = "Handle robokassa result")]
    public async Task<ActionResult<string>> HandleRobokassaResult(
        [FromForm] string OutSum,
        [FromForm] string InvId,
        [FromForm] string EMail,
        [FromForm] string SignatureValue,
        [FromForm] string Shp_userId,
        [FromForm] string Shp_isFirst,
        [FromForm] string Shp_subscriptionId
    )
    {
        var callback = new RobokassaCallback
        {
            InvoiceId = InvId,
            SubscriptionId = Shp_subscriptionId,
            UserId = Shp_userId,
            Signature = SignatureValue,
            OutSum = OutSum,
            IsFirst = bool.Parse(Shp_isFirst)
        };

        try
        {
            await _robokassaPaymentProcessor.VerifyAsync(callback);
        }
        catch (ApplicationException)
        {
            return BadRequest();
        }

        try
        {
            await _robokassaPaymentProcessor.ProcessAsync(callback);

            return Ok($"OK{InvId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"error occured on success ==================> {ex.Message}");
            throw;
        }
    }

    [HttpPost("success", Name = "Success")]
    public IActionResult Success([FromForm] string OutSum,
        [FromForm] string InvId,
        [FromForm] string Shp_subscriptionId)
    {
        return Redirect($"https://web.babytips.me/billing/check-out/{Shp_subscriptionId}");
    }


    [HttpPost("google", Name = "Handle google event")]
    public async Task<ActionResult<string>> Handle(GoogleWebhook webhook)
    {
        var base64Bytes = Convert.FromBase64String(webhook.Message.Data);
        var data = Encoding.UTF8.GetString(base64Bytes);
        var callback = JsonSerializer.Deserialize<GoogleCallback>(data);
        if (callback?.SubscriptionNotification == null) return Ok();
        try
        {
            await _googlePlayBillingProcessor.ProcessAsync(callback);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"error occured on success ==================> {ex.Message}");
            throw;
        }
    }
}