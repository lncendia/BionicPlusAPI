using DomainObjects.Pregnancy.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Services.Interfaces;
using System.Security.Claims;
using PaymentService.Services.Robokassa.Implementations;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RobokassaController : Controller
    {
        private readonly RobokassaClient _robokassaClient;
        private readonly IUserService _userService;
        private readonly RobokassaPaymentProcessor _paymentProcessorService;
        private readonly ILogger<RobokassaController> _logger;

        public RobokassaController(IUserService userService, RobokassaClient robokassaClient, RobokassaPaymentProcessor paymentProcessorService, ILogger<RobokassaController> logger)
        {
            _robokassaClient = robokassaClient;
            _userService = userService;
            _paymentProcessorService = paymentProcessorService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("checkout", Name = "Generate checkout link")]
        public async Task<IActionResult> GenerateCheckoutLink([FromBody] UserAgreement userAgreement, [FromQuery] string planId, [FromQuery] string? promocode)
        {
            if(!userAgreement.PersonalDataAgreement || !userAgreement.RecurringPaymentAgreement || !userAgreement.UserAgreementAgreement)
            {
                return BadRequest("You must allow all user agreements.");
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var saveAgreementsTask = _userService.SaveUserAgreements(userAgreement, userId);
            var linkTask = _robokassaClient.GetCheckoutLink(planId, promocode);

            await Task.WhenAll(saveAgreementsTask, linkTask);

            return Ok(new {
                id = linkTask.Result.subscriptionId,
                link = linkTask.Result.link }) ; 
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
            try
            {
                var sign = _robokassaClient.VerifySignature(SignatureValue, OutSum, InvId, Shp_userId, Shp_isFirst, Shp_subscriptionId); //todo: выноситься в payment procesor

                if (!sign)
                {
                    return BadRequest();
                }

                var isFirst = bool.Parse(Shp_isFirst);

                await _paymentProcessorService.ProcessSuccessPayment(Shp_userId, InvId, isFirst, Shp_subscriptionId);

                return Ok($"OK{InvId}");
            }
            catch(Exception ex)
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
    }
}
