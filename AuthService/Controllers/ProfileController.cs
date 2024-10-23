using AuthService.Dtos;
using AuthService.Models;
using AuthService.Services.Interfaces;
using DomainObjects.Pregnancy.UserProfile;
using DomainObjects.Subscription;
using IdentityLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<ProfileController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ProfileController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ISubscriptionService subscriptionService, ILogger<ProfileController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("", Name = "GetProfile")]
        public async Task<ActionResult<ProfileResponse>> GetProfile()
        {
            var username = HttpContext.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return BadRequest("User not exist");
            };

            var roles = new List<ApplicationRole>(); 

            foreach(var roleId in user.Roles)
            {
                roles.Add(await _roleManager.FindByIdAsync(roleId.ToString()));
            }

            var sub = await _subscriptionService.GetSubscription(user.BillingProfile!.ActiveSubscriptionId);

            if (sub == null)
            {
                _logger.LogError($"An error occurred while getting subscription for user: {user.Id}");
                return NotFound("Subscription not exist");
            }

            var profile = new ProfileResponse
            {
                IsEmailConfirmed = user.EmailConfirmed,
                TemperatureUnits = user.TemperatureUnits,
                MeasureSystem = user.MeasureSystem,
                Height = user.Height,
                Statuses = user.Statuses,
                Email = user.Email,
                UserId = user.Id,
                FullName = user.FullName,
                Roles = roles,
                PlanId = sub.PlanId,
            };

            return Ok(profile);
        }

        [Authorize]
        [HttpGet("plan", Name = "GetPlanInfo")]
        public async Task<ActionResult<PlanResponse>> GetPlanInfo()
        {
            var username = HttpContext.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return BadRequest("User not exist");
            };
            try
            {
                var usage = await _subscriptionService.GetUsage(user.Id.ToString());
                var subscription = await _subscriptionService.GetSubscription(user.BillingProfile?.ActiveSubscriptionId ?? "");

                if (usage == null)
                {
                    _logger.LogError($"An error occurred while getting usage for user: {user.Id}");
                    return NotFound("Usage not exist");
                }

                if (subscription == null)
                {
                    _logger.LogError($"An error occurred while getting subscription for user: {user.Id}");
                    return NotFound("Subscription not exist");
                }

                var plan = await _subscriptionService.GetPlan(subscription.PlanId);

                if (plan == null)
                {
                    _logger.LogError($"An error occurred while getting plan for user: {user.Id}");
                    return NotFound($"Plan {subscription.PlanId} not exist");
                }

                return Ok(new PlanResponse
                {
                    IsFreePlan = user.BillingProfile!.isFreePlan,
                    PlanName = plan.Name,
                    RollbackLimit = usage.RollbackUsage,
                    SurveyLimit = usage.SurveyUsage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting planInfo for user: {user.Id} - {ex}");
                return NotFound("PlanInfo not found");
            }
        }

        [Authorize]
        [HttpPost("units", Name = "SetTemperatureUnits")]
        public async Task<ActionResult<ProfileResponse>> SetTemperatureUnits(TemperatureUnits temperatureUnits)
        {
            var username = HttpContext.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not exist");
            };
            if (user.TemperatureUnits != temperatureUnits)
            {
                user.TemperatureUnits = temperatureUnits;
                await _userManager.UpdateAsync(user);
            }
            return Ok();
        }

        [Authorize]
        [HttpPost("measure", Name = "SetMeasurment")]
        public async Task<ActionResult<ProfileResponse>> SetMeasureSystem(MeasureSystem measureSystem)
        {
            var username = HttpContext.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not exist");
            };
            if (user.MeasureSystem != measureSystem)
            {
                user.MeasureSystem = measureSystem;
                await _userManager.UpdateAsync(user);
            }
            return Ok();
        }

        [Authorize]
        [HttpPost("agreements", Name = "SetUserAgreements")]
        public async Task<ActionResult<ProfileResponse>> SetUserAgreements(UserAgreement userAgreement)
        {
            var username = HttpContext.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not exist");
            };

            if (user.UserAgreement.WhenOccured == null)
            {
                userAgreement.WhenOccured = DateTime.UtcNow;
                user.UserAgreement = userAgreement;
                await _userManager.UpdateAsync(user);
            }

            return Ok();
        }

        [Authorize]
        [HttpPatch("profile", Name = "SetProfileInfo")]
        public async Task<ActionResult<ProfileResponse>> SetProfileInfo(ProfileRequest profile)
        {
            var username = HttpContext.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not exist");
            };
            var updated = false;

            if (profile.FullName != null)
            {
                user.FullName = profile.FullName;
                updated = true;
            }

            if (profile.Height != null)
            {
                user.Height = profile.Height;
                updated = true;
            }

            if (profile.Statuses != null)
            {
                user.Statuses = profile.Statuses;
                updated = true;
            }

            if (updated)
            {
                await _userManager.UpdateAsync(user);
            }

            return Ok();
        }


        [HttpGet("email/unsubscribe", Name = "UnsubscribeEmail")]
        public IActionResult UnsubscribeEmail()
        {
            return Ok("You have successfully unsubscribed from the mailing list.");
        }


        [HttpPost("dnd", Name = "Add user to dnd")]
        public IActionResult AddToDND()
        {
            return Ok("You have been successfully added to the dnd list");
        }

        [HttpDelete(Name = "Delete profile")]
        [Authorize]
        public async Task<IActionResult> DeleteProfile()
        {
            try
            {
                var username = HttpContext.User.Identity!.Name;
                var cancelSubResult = await _subscriptionService.CancelUserSubscription();
                var user = await _userManager.FindByNameAsync(username);
                await _userManager.DeleteAsync(user);
            }
            catch(Exception)
            {
                return BadRequest();
            }
            
            return Ok();
        }
    }
}
