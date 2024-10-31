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
        // if (@event.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Purchased)
        // {
        //     
        // }
        
        return Ok(); 
    }
}