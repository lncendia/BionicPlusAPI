using System.Web;
using AuthService.Configuration;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthService.Services.Implementations;

/// <summary>
/// Класс для валидации капчи
/// </summary>
public class CaptchaValidator : ICaptchaValidator
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
    /// Конструктор класса CaptchaValidator
    /// </summary>
    /// <param name="settingsConfig">Настройки приложения</param>
    /// <param name="httpClient">HTTP-клиент</param>
    public CaptchaValidator(IOptions<SettingsConfig> settingsConfig, HttpClient httpClient)
    {
        _settings = settingsConfig.Value;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Валидирует капчу асинхронно
    /// </summary>
    public async Task<bool> ValidateAsync(string captchaToken)
    {
        try
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

            // Возвращаем результат валидации
            return recaptchaResponse!.Success;
        }
        catch (HttpRequestException)
        {
            // В случае ошибки HTTP-запроса возвращаем false
            return false;
        }
    }
}
