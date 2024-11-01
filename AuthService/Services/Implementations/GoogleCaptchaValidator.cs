using System.Web;
using AuthService.Configuration;
using AuthService.Exceptions;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthService.Services.Implementations;

/// <summary>
/// Класс для валидации капчи
/// </summary>
public class GoogleCaptchaValidator : ICaptchaValidator
{
    /// <summary>
    /// Базовый URL для проверки капчи
    /// </summary>
    private const string BaseUrl = "https://www.google.com/recaptcha/api/siteverify";

    /// <summary>
    /// Настройки приложения
    /// </summary>
    private readonly SettingsConfig _settings;

    /// <summary>
    /// HTTP-клиент для выполнения запросов
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Конструктор класса GoogleCaptchaValidator
    /// </summary>
    /// <param name="settingsConfig">Настройки приложения</param>
    /// <param name="httpClient">HTTP-клиент</param>
    public GoogleCaptchaValidator(IOptions<SettingsConfig> settingsConfig, HttpClient httpClient)
    {
        _settings = settingsConfig.Value;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Валидирует капчу асинхронно
    /// </summary>
    public async Task ValidateAsync(string captchaToken)
    {
        // Создаем строку запроса с параметрами
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["secret"] = _settings.CaptchaSecret;
        query["response"] = captchaToken;

        var queryString = query.ToString();

        // Выполняем POST-запрос к серверу Google reCAPTCHA
        var response = await _httpClient.PostAsync($"{BaseUrl}?{queryString}", null);

        // Читаем ответ от сервера
        var responseBody = await response.Content.ReadAsStringAsync();
        var recaptchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(responseBody);

        ThrowIfErrors(recaptchaResponse);
    }

    /// <summary>
    /// Throws an exception if there are errors in the captcha response.
    /// </summary>
    /// <param name="captchaResponse">The captcha response to validate.</param>
    /// <exception cref="CaptchaException">Thrown if the response is null or contains unspecified errors.</exception>
    /// <exception cref="CaptchaSecretException">Thrown if the response contains secret-related errors.</exception>
    /// <exception cref="CaptchaValidationException">Thrown if the response contains validation-related errors.</exception>
    /// <exception cref="CaptchaResponseException">Thrown if the response contains request-related errors.</exception>
    private static void ThrowIfErrors(CaptchaResponse? captchaResponse)
    {
        // Throw an exception if the captcha response is null
        if (captchaResponse == null) throw new CaptchaException("Response is null");

        // Return if the captcha response indicates success
        if (captchaResponse.Success) return;

        // Throw an exception if the error codes are null
        if (captchaResponse.ErrorCodes == null) throw new CaptchaException("Response is null");

        // Create a string representation of the error codes
        var errorMessage = string.Join(", ", captchaResponse.ErrorCodes);

        // Throw a CaptchaSecretException if the error codes contain secret-related errors
        if (captchaResponse.ErrorCodes.Contains("missing-input-secret") ||
            captchaResponse.ErrorCodes.Contains("invalid-input-secret"))
        {
            throw new CaptchaSecretException(errorMessage);
        }

        // Throw a CaptchaValidationException if the error codes contain validation-related errors
        if (captchaResponse.ErrorCodes.Contains("missing-input-response") ||
            captchaResponse.ErrorCodes.Contains("invalid-input-response"))
        {
            throw new CaptchaValidationException(errorMessage);
        }

        // Throw a CaptchaResponseException if the error codes contain request-related errors
        if (captchaResponse.ErrorCodes.Contains("bad-request") ||
            captchaResponse.ErrorCodes.Contains("timeout-or-duplicate"))
        {
            throw new CaptchaResponseException(errorMessage);
        }

        // Throw a general CaptchaException for any other errors
        throw new CaptchaException(errorMessage);
    }
}