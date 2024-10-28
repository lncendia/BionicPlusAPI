using AuthService.Models;
using AuthService.Services.Implementations;
using AuthService.Services.Interfaces;
using DomainObjects.Pregnancy.Localizations;
using DomainObjects.Subscription;
using MailSenderLibrary.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using IdentityLibrary;
using Microsoft.Extensions.Options;

namespace AuthService.Controllers;

/// <summary>
/// Контроллер для аутентификации пользователей.
/// </summary>
[ApiController]
[Route("api/v1/authenticate")]
public class AuthenticationController : ControllerBase
{
    /// <summary>
    /// Менеджер пользователей.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;
    
    /// <summary>
    /// Конфигурация Identity.
    /// </summary>
    private readonly IdentityOptions _identityOptions;

    /// <summary>
    /// Сервис для отправки электронной почты.
    /// </summary>
    private readonly IEmailService _emailService;

    /// <summary>
    /// Сервис для работы с JWT токенами.
    /// </summary>
    private readonly IJwtService _jwtService;

    /// <summary>
    /// Сервис валидации капчи.
    /// </summary>
    private readonly ICaptchaValidator _captchaValidator;

    /// <summary>
    /// Фабрика для создания объектов ClaimsPrincipal.
    /// </summary>
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    /// <summary>
    /// Сервис для управления подписками.
    /// </summary>
    private readonly ISubscriptionService _subscriptionService;

    /// <summary>
    /// Сервис управления внешними OIDC-провайдерами.
    /// </summary>
    private readonly IExternalOidcManager _externalOidcManager;

    /// <summary>
    /// Логгер для логирования событий.
    /// </summary>
    private readonly ILogger<AuthenticationController> _logger;

    /// <summary>
    /// Доступ к HTTP контексту.
    /// </summary>
    private readonly IHttpContextAccessor _context;

    /// <summary>
    /// Конструктор контроллера аутентификации.
    /// </summary>
    /// <param name="userManager">Менеджер пользователей.</param>
    /// <param name="emailService">Сервис для отправки электронной почты.</param>
    /// <param name="subscriptionService">Сервис для управления подписками.</param>
    /// <param name="httpContextAccessor">Доступ к HTTP контексту.</param>
    /// <param name="logger">Логгер для логирования событий.</param>
    /// <param name="jwtService">Сервис для работы с JWT токенами.</param>
    /// <param name="userClaimsPrincipalFactory">Фабрика для создания объектов ClaimsPrincipal.</param>
    /// <param name="captchaValidator">Сервис валидации капчи.</param>
    /// <param name="identityOptions">Конфигурация Identity.</param>
    /// <param name="externalOidcManager">Сервис управления внешними OIDC-провайдерами.</param>
    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ISubscriptionService subscriptionService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationController> logger,
        IJwtService jwtService,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, 
        ICaptchaValidator captchaValidator, 
        IOptions<IdentityOptions> identityOptions,
        IExternalOidcManager externalOidcManager)
    {
        _userManager = userManager;
        _emailService = emailService;
        _subscriptionService = subscriptionService;
        _logger = logger;
        _jwtService = jwtService;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _captchaValidator = captchaValidator;
        _identityOptions = identityOptions.Value;
        _externalOidcManager = externalOidcManager;
        _context = httpContextAccessor;
    }

    /// <summary>
    /// Метод для запроса на восстановление пароля.
    /// </summary>
    /// <param name="req">Объект, содержащий email и токен капчи.</param>
    /// <returns>Ответ с сообщением об успешной отправке или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("forgot/password/")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
    {
        try
        {
            // Проверка капчи
            var isCaptchaValid = await _captchaValidator.ValidateAsync(req.Captcha);

            // Если капча не пройдена, возвращаем ошибку
            if (!isCaptchaValid)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Captcha was not validated",
                    Code = AuthErrorCode.CaptchaNotPassed
                });
            }

            // Поиск пользователя по email
            var user = await _userManager.FindByEmailAsync(req.Email);

            // Генерация токена для сброса пароля
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Отправка email с токеном для сброса пароля
            await _emailService.SendEmailAsync(MailGenerator.GenerateTokenMessage(token, user.Email, LocalizationsLanguage.en));

            // Возвращаем успешный ответ
            return Ok();
        }
        catch (Exception ex)
        {
            // Логирование ошибки и возврат сообщения об ошибке
            _logger.LogError(ex, "Произошла ошибка при запросе на восстановление пароля");
            return BadRequest();
        }
    }

    /// <summary>
    /// Метод для восстановления пароля.
    /// </summary>
    /// <param name="req">Объект, содержащий email, токен и новый пароль.</param>
    /// <returns>Ответ с сообщением об успешном восстановлении или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("recover/password")]
    public async Task<IActionResult> RecoverPassword(RecoverPasswordRequest req)
    {
        // Поиск пользователя по email
        var user = await _userManager.FindByEmailAsync(req.Email);

        // Сброс пароля пользователя с использованием токена
        var result = await _userManager.ResetPasswordAsync(user, req.Token, req.NewPassword);

        // Если сброс пароля не удался, возвращаем ошибку
        if (!result.Succeeded)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = $"The password could not be restored {result.Errors?.FirstOrDefault()?.Description}",
                Code = Enum.Parse<AuthErrorCode>(result.Errors?.FirstOrDefault()?.Code ?? "")
            });
        }

        // Возвращаем успешный ответ
        return Ok();
    }

    /// <summary>
    /// Метод для изменения пароля текущего пользователя.
    /// </summary>
    /// <param name="req">Объект, содержащий старый и новый пароль.</param>
    /// <returns>Ответ с сообщением об успешном изменении или сообщение об ошибке.</returns>
    [Authorize]
    [HttpPost]
    [Route("change/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
    {
        // Получение идентификатора текущего пользователя из контекста запроса
        var userId = _context.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Поиск пользователя по идентификатору
        var user = await _userManager.FindByIdAsync(userId);

        // Изменение пароля пользователя
        var result = await _userManager.ChangePasswordAsync(user, req.OldPassword, req.NewPassword);

        // Если изменение пароля не удалось, возвращаем ошибку
        if (!result.Succeeded)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = $"The password could not be restored {result.Errors?.FirstOrDefault()?.Description}",
                Code = Enum.Parse<AuthErrorCode>(result.Errors?.FirstOrDefault()?.Code ?? "")
            });
        }

        // Возвращаем успешный ответ
        return Ok();
    }


    /// <summary>
    /// Метод для регистрации нового пользователя.
    /// </summary>
    /// <param name="register">Объект, содержащий данные для регистрации пользователя.</param>
    /// <returns>Ответ с сообщением об успешной регистрации или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(typeof(RegisterResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest register)
    {
        // Создаем переменную для хранения пользователя
        ApplicationUser? user = null;

        try
        {
            // Проверка капчи
            var isCaptchaValid = await _captchaValidator.ValidateAsync(register.Captcha);

            // Если капча не пройдена, возвращаем ошибку
            if (!isCaptchaValid)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Captcha was not validated",
                    Code = AuthErrorCode.CaptchaNotPassed
                });
            }

            // Поиск пользователя по email
            user = await _userManager.FindByEmailAsync(register.Email);

            // Если пользователь уже существует, возвращаем ошибку
            if (user != null)
            {
                return BadRequest(new RegisterResponse { Success = false, Message = "The user already exists" });
            }

            // Создание нового пользователя
            user = new ApplicationUser
            {
                Email = register.Email,
                UserName = register.Email
            };

            // Создание пользователя в системе
            var createUserResult = await _userManager.CreateAsync(user, register.Password);

            // Если создание пользователя не удалось, возвращаем ошибку
            if (!createUserResult.Succeeded)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message =
                        $"Failed to create a user {createUserResult.Errors?.FirstOrDefault()?.Description}",
                    Code = Enum.Parse<AuthErrorCode>(createUserResult.Errors?.FirstOrDefault()?.Code ?? "")
                });
            }

            // Создание объекта principal для пользователя
            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            // Генерация токена доступа
            var token = _jwtService.GenerateAccessToken(principal, Guid.NewGuid());

            // Добавление токена доступа в заголовки запроса
            _context.HttpContext!.Request.Headers.Add("Authorization", $"Bearer {token}");

            // Установка бесплатной подписки для пользователя
            var subscriptionId = await _subscriptionService.SetFreeSubscription(user.Id.ToString());

            // Если установка подписки не удалась, удаляем пользователя и возвращаем ошибку
            if (subscriptionId == null)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Failed to create a user. Failed to install subscription"
                });
            }

            // Установка профиля биллинга для пользователя
            user.BillingProfile = new BillingProfile
            {
                ActiveSubscriptionId = subscriptionId,
                isFreePlan = true,
                SubscriptionIds = new List<string> { subscriptionId }
            };

            // Обновление информации о пользователе в базе данных
            await _userManager.UpdateAsync(user);

            // Добавление пользователя в роль "USER"
            var addUserToRole = await _userManager.AddToRoleAsync(user, "USER");

            // Если добавление пользователя в роль не удалось, удаляем пользователя и возвращаем ошибку
            if (!addUserToRole.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message =
                        $"The user could not be added to the role {addUserToRole.Errors?.FirstOrDefault()?.Description}"
                });
            }

            // Генерация токена подтверждения email
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Отправка email с токеном подтверждения
            await _emailService.SendEmailAsync(
                MailGenerator.GenerateTokenMessage(confirmToken, user.Email, LocalizationsLanguage.en));

            // Возвращаем успешный ответ
            return Ok(new RegisterResponse { Success = true, Message = "The user has been successfully created" });
        }
        catch (Exception ex)
        {
            // Если пользователь был создан - удаляем его
            if (user != null) await _userManager.DeleteAsync(user);

            // Логирование ошибки и возврат сообщения об ошибке
            _logger.LogError(ex, "Произошла ошибка при регистрации");
            return BadRequest(new RegisterResponse { Success = false, Message = ex.Message });
        }
    }


    /// <summary>
    /// Метод для обработки запроса на вход в систему.
    /// </summary>
    /// <param name="login">Объект, содержащий email и пароль пользователя.</param>
    /// <returns>Ответ с токенами доступа и обновления, если вход успешен, или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest login)
    {
        try
        {
            // Поиск пользователя по email
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                // Если пользователь не найден, возвращаем ошибку
                return BadRequest(new LoginResponse
                {
                    Success = false, Message = "The user does not exist", Code = AuthErrorCode.UserNotExists
                });
            }

            // Проверка пароля пользователя
            if (!await _userManager.CheckPasswordAsync(user, login.Password))
            {
                // Если пароль неверен, возвращаем ошибку
                return BadRequest(new LoginResponse
                    { Success = false, Message = "Invalid password", Code = AuthErrorCode.PasswordIncorrect });
            }

            // Создание объекта principal для пользователя
            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
            
            // Создаем идентификатор токена
            var jti = Guid.NewGuid();
            
            // Генерация токена доступа
            var token = _jwtService.GenerateAccessToken(principal, jti);

            // Генерация токена обновления и времени его истечения
            var (refreshToken, refreshTokenExpiryTime) = _jwtService.GenerateRefreshToken(jti);
            
            // Возвращаем успешный ответ с токенами и информацией о пользователе
            return Ok(new LoginResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Message = "The login was completed successfully",
                Email = user.Email,
                Success = true,
                UserId = user.Id,
                RefreshTokenExpiryTime = refreshTokenExpiryTime
            });
        }
        catch (Exception ex)
        {
            // Логирование ошибки и возврат сообщения об ошибке
            _logger.LogError(ex, "Произошла ошибка при входе в систему");
            return BadRequest(new LoginResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Метод для обработки запроса на вход в систему.
    /// </summary>
    /// <param name="login">Объект, содержащий имя провайдера аутентификации и токен идентификации.</param>
    /// <returns>Ответ с токенами доступа и обновления, если вход успешен, или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("login/external")]
    [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> LoginExternal([FromBody] LoginExternalRequest login)
    {
        // Создаем переменную для хранения пользователя
        ApplicationUser? user = null;
        
        // Создаем флаг, для того чтобы понимать, новый ли это пользователь
        var inNewUser = false;
        
        try
        {
            // Расшифровываем и проверяем токен идентификации
            var externalInfo = await _externalOidcManager.GetLoginInfoAsync(login.Provider, login.IdentityToken);

            // Пытаемся получить пользователя по логину
            user = await _userManager.FindByLoginAsync(externalInfo.LoginProvider, externalInfo.ProviderKey);

            // Если пользователь еще не зарегистрирован - устанавливаем флаг, что ему необходимо установить подписку
            inNewUser = user == null;

            // Создаем нового пользователя, если он еще не зарегистрирован
            if (user == null)
            {
                // Пытаемся получить почту из утверждений, если почты нет - вызываем исключение
                var email = externalInfo.Principal.FindFirstValue(ClaimTypes.Email);

                // Если почта пользователя не найдена, возвращаем ошибку
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new RegisterResponse
                    {
                        Success = false,
                        Message = "Внешний провайдер аутентификации не предоставил почту пользователя",
                        Code = AuthErrorCode.InvalidToken
                    });
                }

                // Создание нового пользователя
                user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                // Создание пользователя в системе
                var createUserResult = await _userManager.CreateAsync(user);

                // Если создание пользователя не удалось, возвращаем ошибку
                if (!createUserResult.Succeeded)
                {
                    return BadRequest(new RegisterResponse
                    {
                        Success = false,
                        Message =
                            $"Не удалось создать пользователя {createUserResult.Errors.FirstOrDefault()?.Description}",
                        Code = Enum.Parse<AuthErrorCode>(createUserResult.Errors.FirstOrDefault()?.Code ?? "")
                    });
                }

                // Связываем пользователя с внешним провайдером
                await _userManager.AddLoginAsync(user, externalInfo);
            }

            // Создание объекта principal для пользователя
            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            // Получение данных запроса
            var request = _context.HttpContext!.Request;

            // Формируем издателя токена
            var issuer = $"{request.Scheme}://{request.Host.Value}";

            // Генерация токена доступа
            var token = _jwtService.GenerateAccessToken(principal, issuer);

            // Генерация токена обновления и времени его истечения
            var (refreshToken, refreshTokenExpiryTime) = _jwtService.GenerateRefreshToken();

            // Устанавливаем новый токен обновления для пользователя
            user.RefreshToken = refreshToken;

            // Устанавливаем время истечения токена обновления
            user.RefreshTokenExpiryTime = DateTime.Now.Add(refreshTokenExpiryTime);

            // Устанавливаем подписку, если это необходимо
            if (inNewUser)
            {
                // Добавление токена доступа в заголовки запроса
                _context.HttpContext!.Request.Headers.Add("Authorization", $"Bearer {token}");

                // Установка бесплатной подписки для пользователя
                var subscriptionId = await _subscriptionService.SetFreeSubscription(user.Id.ToString());

                // Если установка подписки не удалась, удаляем пользователя и возвращаем ошибку
                if (subscriptionId == null)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new RegisterResponse
                    {
                        Success = false,
                        Message = "Не удалось создать пользователя. Не удалось установить подписку"
                    });
                }

                // Установка профиля биллинга для пользователя
                user.BillingProfile = new BillingProfile
                {
                    ActiveSubscriptionId = subscriptionId,
                    isFreePlan = true,
                    SubscriptionIds = new List<string> { subscriptionId }
                };

                // Добавление пользователя в роль "USER"
                var addUserToRole = await _userManager.AddToRoleAsync(user, "USER");

                // Если добавление пользователя в роль не удалось, удаляем пользователя и возвращаем ошибку
                if (!addUserToRole.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return BadRequest(new RegisterResponse
                    {
                        Success = false,
                        Message =
                            $"Не удалось добавить пользователя в роль {addUserToRole.Errors?.FirstOrDefault()?.Description}"
                    });
                }
            }

            // Обновляем информацию о пользователе в базе данных
            await _userManager.UpdateAsync(user);

            // Возвращаем успешный ответ с токенами и информацией о пользователе
            return Ok(new LoginResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Message = "Вход выполнен успешно",
                Email = user.Email,
                Success = true,
                UserId = user.Id,
                RefreshTokenExpiryTime = (int)refreshTokenExpiryTime.TotalDays
            });
        }
        catch (Exception ex)
        {
            // Если пользователь был создан - удаляем его
            if (user != null && inNewUser) await _userManager.DeleteAsync(user);
            
            // Логирование ошибки и возврат сообщения об ошибке
            _logger.LogError(ex, "Произошла ошибка при входе в систему");
            return BadRequest(new LoginResponse { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Метод для обновления токена доступа.
    /// </summary>
    /// <param name="tokenRequest">Объект, содержащий текущие токены доступа и обновления.</param>
    /// <returns>Обновленные токены доступа и обновления, если запрос успешен, или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
    {
        try
        {
            // Получение principal из истекшего токена доступа
            var principal = _jwtService.GetPrincipalFromExpiredToken(tokenRequest.AccessToken, tokenRequest.RefreshToken);

            // Получение идентификатора пользователя из principal
            var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            // Получение security stamp из principal
            var securityStamp = principal.FindFirstValue(_identityOptions.ClaimsIdentity.SecurityStampClaimType);

            // Поиск пользователя по идентификатору
            var user = await _userManager.FindByIdAsync(id);
            
            // Если пользователь не прошел проверку безопасности, возвращаем ошибку с сообщением
            if (user == null || user.SecurityStamp != securityStamp)
            {
                return BadRequest(new LoginResponse {
                    Success = false,
                    Message = "The security stamp is outdated. Please authenticate again",
                    Code = AuthErrorCode.SecurityStampOutdated
                });
            }
            
            // Создаем идентификатор токена
            var jti = Guid.NewGuid();
            
            // Генерация нового токена доступа
            var accessToken = _jwtService.GenerateAccessToken(principal, jti);

            // Генерация нового токена обновления и времени его истечения
            var (refreshToken, refreshTokenExpiryTime) = _jwtService.GenerateRefreshToken(jti);

            // Возвращаем обновленные токены
            return Ok(new RefreshResponse
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime
            });
        }
        catch (Exception ex)
        {
            // Логирование ошибки и возврат сообщения об ошибке
            _logger.LogError(ex, "Произошла ошибка при обновлении токена");
            return BadRequest(new LoginResponse { Success = false, Message = ex.Message });
        }
    }


    /// <summary>
    /// Метод для проверки существования пользователя по email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Ответ с информацией о статусе email.</returns>
    [HttpGet]
    [Route("check")]
    public async Task<ActionResult<CheckEmailResponse>> CheckUserByEmail(string email)
    {
        // Поиск пользователя по email
        var user = await _userManager.FindByEmailAsync(email);

        // Если пользователь не найден, возвращаем статус "NotFound"
        if (user == null)
        {
            return Ok(new CheckEmailResponse { EmailStatus = EmailStatus.NotFound });
        }

        // Возвращаем статус email в зависимости от того, подтвержден ли он
        return Ok(user.EmailConfirmed == false
            ? new CheckEmailResponse { EmailStatus = EmailStatus.NotConfirmed }
            : new CheckEmailResponse { EmailStatus = EmailStatus.Created });
    }
    

    /// <summary>
    /// Метод для подтверждения email пользователя.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <param name="token">Токен подтверждения email.</param>
    /// <returns>Ответ с сообщением об успешном подтверждении или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("confirm/email")]
    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        // Поиск пользователя по email
        var user = await _userManager.FindByEmailAsync(email);

        // Если пользователь не найден, возвращаем ошибку
        if (user == null)
        {
            return NotFound("The user was not found");
        }

        // Если email уже подтвержден, возвращаем ошибку
        if (user.EmailConfirmed)
        {
            return BadRequest("The user's email has already been confirmed");
        }

        // Подтверждение email пользователя с использованием токена
        var confirmResult = await _userManager.ConfirmEmailAsync(user, token);

        // Если подтверждение не удалось, возвращаем ошибку
        if (confirmResult.Succeeded != true)
        {
            return BadRequest("Token confirmation error");
        }

        // Возвращаем успешный ответ
        return Ok();
    }
    
    /// <summary>
    /// Метод для закрытия всех сессий.
    /// </summary>
    /// <returns>Ответ без содержимого, если операция успешна, или сообщение об ошибке.</returns>
    [Authorize]
    [HttpPost]
    [Route("revokeAll")]
    public async Task<IActionResult> RevokeAll()
    {
        // Получение имени текущего пользователя из контекста запроса
        var username = HttpContext.User.Identity!.Name;

        // Поиск пользователя по имени
        var user = await _userManager.FindByNameAsync(username);

        // Если пользователь не найден, возвращаем ошибку
        if (user == null)
        {
            return BadRequest("Пользователь не существует");
        }

        // Обновление SecurityStamp у пользователя
        await _userManager.UpdateSecurityStampAsync(user);

        // Возвращаем ответ без содержимого
        return NoContent();
    }

    /// <summary>
    /// Метод для повторной отправки токена подтверждения email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <returns>Ответ с сообщением об успешной отправке или сообщение об ошибке.</returns>
    [HttpPost]
    [Route("resend")]
    public async Task<IActionResult> ResendConfirmationToken(string email)
    {
        // Поиск пользователя по email
        var user = await _userManager.FindByEmailAsync(email);

        // Если пользователь не найден, возвращаем ошибку
        if (user == null)
        {
            return NotFound("The user was not found");
        }

        // Проверка, подтвержден ли уже email пользователя
        if (user.EmailConfirmed)
        {
            // Если email уже подтвержден, возвращаем ошибку
            return BadRequest("The user's email has already been confirmed");
        }

        // Генерация нового токена подтверждения email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Отправка email с новым токеном подтверждения
        await _emailService.SendEmailAsync(MailGenerator.GenerateTokenMessage(token, email, LocalizationsLanguage.en));

        // Возвращаем успешный ответ
        return Ok();
    }
}