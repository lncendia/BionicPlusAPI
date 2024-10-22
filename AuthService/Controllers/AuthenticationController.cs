using AspNetCore.Identity.MongoDbCore.Models;
using AuthService.Dtos;
using AuthService.Infrastructure;
using AuthService.Models;
using AuthService.Services.Implementations;
using AuthService.Services.Interfaces;
using DnsClient;
using DomainObjects.Pregnancy.Localizations;
using DomainObjects.Subscription;
using IdentityLibrary.Models;
using MailSenderLibrary.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/v1/authenticate")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IEmailService _emailService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IHttpContextAccessor _context;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<JwtConfig> jwtConfig,
            TokenValidationParameters tokenValidationParameters,
            IEmailService emailService,
            ISubscriptionService subscriptionService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtConfig = jwtConfig.Value;
            _tokenValidationParameters = tokenValidationParameters;
            _emailService = emailService;
            _subscriptionService = subscriptionService;
            _logger = logger;
            _context = httpContextAccessor;
        }


        ////use this method if it necessary to add new role
        //[HttpPost]
        //[Route("roles/add")]
        //public async Task<IActionResult> CreateRole([FromBody] RoleRequest roleRequest)
        //{
        //    var appRole = new ApplicationRole { Name = roleRequest.Role };
        //    var createRole = await _roleManager.CreateAsync(appRole);
        //    return Ok(new { Message = $"Role {roleRequest.Role} created successfully" });
        //}

        [HttpPost]
        [Route("forgot/password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                _emailService.SendEmail(MailGenerator.GenerateTokenMessage(token, user.Email, LocalizationsLanguage.en));
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while register: {ex}");
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("recover/password")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);

            var result = await _userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = $"Recover password failed {result?.Errors?.FirstOrDefault()?.Description}",
                    Code = Enum.Parse<AuthErrorCode>(result?.Errors?.FirstOrDefault()?.Code ?? "")
                });
            }
            
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("change/password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            var userId = _context.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user, req.OldPassword, req.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = $"Change password failed {result?.Errors?.FirstOrDefault()?.Description}",
                    Code = Enum.Parse<AuthErrorCode>(result?.Errors?.FirstOrDefault()?.Code ?? "")
                });
            }

            return Ok();
        }


        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(RegisterResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            var result = await RegisterAsync(register);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var result = await LoginAsync(login);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;
            try
            {
                var principal = GetPrincipalFromExpiredToken(accessToken);
                if (principal == null)
                {
                    return BadRequest("Invalid access token");
                }

                string username = principal.Identity!.Name!;


                var user = await _userManager.FindByNameAsync(username);

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return BadRequest("Invalid access token or refresh token");
                }

                var newAccessToken = await GenerateAccessToken(user);
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                await _userManager.UpdateAsync(user);

                return new ObjectResult(new
                {
                    accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                    refreshToken = newRefreshToken,
                    RefreshTokenExpiryTime = _jwtConfig.RefreshTokenValidityInDays,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while refresh: {ex}");
                return BadRequest("Invalid client request");
            }
        }

        [HttpGet]
        [Route("check")]
        public async Task<ActionResult<CheckEmailResponse>> CheckUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Ok(new CheckEmailResponse { EmailStatus = EmailStatus.NotFound });
            }
            if(user != null && user.EmailConfirmed == false)
            {
                return Ok(new CheckEmailResponse { EmailStatus = EmailStatus.NotConfirmed });
            }
            

            return Ok(new CheckEmailResponse { EmailStatus = EmailStatus.Created });
        }

        [Authorize]
        [HttpPost]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = HttpContext.User.Identity!.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User not exist");
            };

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpPost]
        [Route("confirm/email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return NotFound("User not found");
            }
            if(user != null && user.EmailConfirmed)
            {
                return BadRequest("Users email already confirmed");
            }

            var confirmResult = await _userManager.ConfirmEmailAsync(user!, token);
            if(confirmResult.Succeeded != true)
            {
                return BadRequest("Token confirmation faulted");
            }

            return Ok();
        }

        [HttpPost]
        [Route("resend")]
        public async Task<IActionResult> ResendConfirmationToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (user != null && user.EmailConfirmed)
            {
                return BadRequest("Users email already confirmed");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
            _emailService.SendEmail(MailGenerator.GenerateTokenMessage(token, email, LocalizationsLanguage.en));
            return Ok();

        }

        [NonAction]
        private async Task<RegisterResponse> RegisterAsync(RegisterRequest register)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(register.Email);
                if (userExist != null)
                {
                    return new RegisterResponse { Success = false, Message = "User already exist" };
                }

                userExist = new ApplicationUser
                {
                    Email = register.Email,
                    UserName = register.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                var createUserResult = await _userManager.CreateAsync(userExist, register.Password);

                if (!createUserResult.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(userExist.Email);
                    if(user != null)
                    {
                        await _userManager.DeleteAsync(user);
                    }
                    return new RegisterResponse { Success = false, 
                                                  Message = $"Create user failed {createUserResult?.Errors?.FirstOrDefault()?.Description}",
                                                  Code = Enum.Parse<AuthErrorCode>(createUserResult?.Errors?.FirstOrDefault()?.Code ?? "")};
                }

                var userForToken = await _userManager.FindByEmailAsync(userExist.Email);

                var subscriptionId = await SetFreeSubscription(register.Email);

                if (subscriptionId == null)
                {
                    var user = await _userManager.FindByEmailAsync(userExist.Email);
                    await _userManager.DeleteAsync(user);
                    return new RegisterResponse { Success = false, Message = $"Create user failed. Can not set subscription" };
                }

                userExist.BillingProfile = new BillingProfile { ActiveSubscriptionId = subscriptionId, isFreePlan = true, SubscriptionIds = new List<string> { subscriptionId } };
                await _userManager.UpdateAsync(userExist);

                
                var addUserToRole = await _userManager.AddToRoleAsync(userExist, "USER");

                if (!addUserToRole.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(userExist.Email);
                    await _userManager.DeleteAsync(user);
                    return new RegisterResponse { Success = false, Message = $"Could not add user to role {addUserToRole?.Errors?.FirstOrDefault()?.Description}" };
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(userExist);
                _emailService.SendEmail(MailGenerator.GenerateTokenMessage(token, userExist.Email, LocalizationsLanguage.en));

                return new RegisterResponse { Success = true, Message = "User created successfully" };
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while register: {ex}");
                var user = await _userManager.FindByEmailAsync(register.Email);
                if(user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
                return new RegisterResponse { Success = false, Message = ex.Message };
            }
        }


        [NonAction]
        private async Task<string?> SetFreeSubscription(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessToken = await GenerateAccessToken(user);

            _context.HttpContext!.Request.Headers.Add("Authorization", $"Bearer {new JwtSecurityTokenHandler().WriteToken(accessToken)}");

            var subscriptionId = await _subscriptionService.SetFreeSubscription(user.Id.ToString());
           
            return subscriptionId;
        }

        [NonAction]
        private async Task<LoginResponse> LoginAsync(LoginRequest login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user == null)
                {
                    return new LoginResponse { Success = false, Message = "User not exists", Code = AuthErrorCode.UserNotExists};
                }

                if(!await _userManager.CheckPasswordAsync(user, login.Password))
                {
                    return new LoginResponse { Success = false, Message = "Password incorrect", Code = AuthErrorCode.PasswordIncorrect };
                }


                var token = await GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                var expirationTime = DateTime.Now.AddDays(_jwtConfig.RefreshTokenValidityInDays);
                user.RefreshTokenExpiryTime = expirationTime;

                await _userManager.UpdateAsync(user);
                return new LoginResponse
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Message = "Login Success",
                    Email = user.Email,
                    Success = true,
                    UserId = user.Id.ToString(),
                    RefreshTokenExpiryTime = _jwtConfig.RefreshTokenValidityInDays,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while login: {ex}");
                return new LoginResponse { Success = false, Message = ex.Message };
            }
        }

        [NonAction]
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        [NonAction]
        private async Task<JwtSecurityToken> GenerateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaims);
            var expires = DateTime.Now.AddHours(3);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.IssuerSigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: _jwtConfig.ValidIssuer,
                audience: _jwtConfig.ValidAudiences.Split(",").FirstOrDefault(),
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return token;
        }

        [NonAction]
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            _tokenValidationParameters.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch(ArgumentException ex)
            {
                _logger.LogError($"An error occurred while GetPrincipalFromExpiredToken: {ex}");
                throw new SecurityTokenException("Invalid token");
            }
        }
    }
}
