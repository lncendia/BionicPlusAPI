using System.Web;
using AuthService.Configuration;
using AuthService.Dtos;
using AuthService.Exceptions;
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

    private static void ThrowIfErrors(CaptchaResponse? captchaResponse)
    {
        if (captchaResponse == null) throw new CaptchaException("Response is null");

        if (captchaResponse.Success) return;
        
        if (captchaResponse.ErrorCodes == null) throw new CaptchaException("Response is null");

        var errorMessage = string.Join(", ", captchaResponse.ErrorCodes);

        if (captchaResponse.ErrorCodes.Contains("missing-input-secret") ||
            captchaResponse.ErrorCodes.Contains("invalid-input-secret"))
        {
            throw new CaptchaSecretException(errorMessage);
        }

        if (captchaResponse.ErrorCodes.Contains("missing-input-response") ||
            captchaResponse.ErrorCodes.Contains("invalid-input-response"))
        {
            throw new CaptchaValidationException(errorMessage);
        }

        if (captchaResponse.ErrorCodes.Contains("bad-request") ||
            captchaResponse.ErrorCodes.Contains("timeout-or-duplicate"))
        {
            throw new CaptchaResponseException(errorMessage);
        }

        throw new CaptchaException(errorMessage);
    }
}