using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

/// <summary>
/// Конфигурация клиентских конечных точек.
/// </summary>
public class ClientEndpointsConfig
{
    /// <summary>
    /// Конфигурация конечных точек.
    /// </summary>
    private readonly EndpointsConfig _endpointsConfig;

    /// <summary>
    /// Конфигурация обратного прокси.
    /// </summary>
    private readonly ReverseProxyConfig _reverseProxyConfig;

    /// <summary>
    /// Конструктор класса ClientEndpointsConfig.
    /// </summary>
    /// <param name="endpointsConfig">Конфигурация конечных точек.</param>
    /// <param name="reverseProxyConfig">Конфигурация обратного прокси.</param>
    public ClientEndpointsConfig(EndpointsConfig endpointsConfig, ReverseProxyConfig reverseProxyConfig)
    {
        _endpointsConfig = endpointsConfig;
        _reverseProxyConfig = reverseProxyConfig;
    }

    /// <summary>
    /// Рассчитывает URL с использованием прокси.
    /// </summary>
    /// <param name="url">Оригинальный URL.</param>
    /// <param name="proxy">URL прокси.</param>
    /// <returns>Новый URL с использованием прокси.</returns>
    private static string CalculateUrl(string url, string proxy)
    {
        // Если прокси пуст, возвращаем оригинальный URL
        if (string.IsNullOrEmpty(proxy)) return url;

        // Если оригинальный URL пуст, возвращаем его
        if (string.IsNullOrEmpty(url)) return url;

        // Разбираем оригинальный URL
        var originalUri = new Uri(url);

        // Разбираем новый базовый URL
        var newBaseUri = new Uri(proxy);

        // Создаем новый URL с использованием пути оригинального URL и нового базового URL
        return newBaseUri.ToString().TrimEnd('/') + originalUri.PathAndQuery;
    }

    /// <summary>
    /// URL для проверки аккаунта.
    /// </summary>
    [JsonPropertyName("checkAccountUrl")]
    public string CheckAccountUrl => CalculateUrl(_endpointsConfig.CheckAccountUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для входа.
    /// </summary>
    [JsonPropertyName("loginUrl")]
    public string LoginUrl => CalculateUrl(_endpointsConfig.LoginUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для регистрации.
    /// </summary>
    [JsonPropertyName("registerUrl")]
    public string RegisterUrl => CalculateUrl(_endpointsConfig.RegisterUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для обновления токенов.
    /// </summary>
    [JsonPropertyName("refreshTokensUrl")]
    public string RefreshTokensUrl => CalculateUrl(_endpointsConfig.RefreshTokensUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для отзыва токенов.
    /// </summary>
    [JsonPropertyName("revokeTokensUrl")]
    public string RevokeTokensUrl => CalculateUrl(_endpointsConfig.RevokeTokensUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для подтверждения email.
    /// </summary>
    [JsonPropertyName("confirmEmailUrl")]
    public string ConfirmEmailUrl => CalculateUrl(_endpointsConfig.ConfirmEmailUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для повторной отправки кода.
    /// </summary>
    [JsonPropertyName("resendCodeUrl")]
    public string ResendCodeUrl => CalculateUrl(_endpointsConfig.ResendCodeUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для восстановления пароля.
    /// </summary>
    [JsonPropertyName("forgotPasswordUrl")]
    public string ForgotPasswordUrl => CalculateUrl(_endpointsConfig.ForgotPasswordUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для восстановления пароля.
    /// </summary>
    [JsonPropertyName("recoverPasswordUrl")]
    public string RecoverPasswordUrl => CalculateUrl(_endpointsConfig.RecoverPasswordUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для изменения пароля.
    /// </summary>
    [JsonPropertyName("changePasswordUrl")]
    public string ChangePasswordUrl => CalculateUrl(_endpointsConfig.ChangePasswordUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для списка карт.
    /// </summary>
    [JsonPropertyName("cardListUrl")]
    public string CardListUrl => CalculateUrl(_endpointsConfig.CardListUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для карты.
    /// </summary>
    [JsonPropertyName("cardUrl")]
    public string CardUrl => CalculateUrl(_endpointsConfig.CardUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для базовой карты.
    /// </summary>
    [JsonPropertyName("basicCardUrl")]
    public string BasicCardUrl => CalculateUrl(_endpointsConfig.BasicCardUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для загрузки изображения.
    /// </summary>
    [JsonPropertyName("imageUploadUrl")]
    public string ImageUploadUrl => CalculateUrl(_endpointsConfig.ImageUploadUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для профиля.
    /// </summary>
    [JsonPropertyName("profileUrl")]
    public string ProfileUrl => CalculateUrl(_endpointsConfig.ProfileUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для обновления профиля.
    /// </summary>
    [JsonPropertyName("profileUpdateUrl")]
    public string ProfileUpdateUrl => CalculateUrl(_endpointsConfig.ProfileUpdateUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для единиц профиля.
    /// </summary>
    [JsonPropertyName("profileUnitsUrl")]
    public string ProfileUnitsUrl => CalculateUrl(_endpointsConfig.ProfileUnitsUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для измерений профиля.
    /// </summary>
    [JsonPropertyName("profileMeasureUrl")]
    public string ProfileMeasureUrl => CalculateUrl(_endpointsConfig.ProfileMeasureUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL для опроса.
    /// </summary>
    [JsonPropertyName("surveyUrl")]
    public string SurveyUrl => CalculateUrl(_endpointsConfig.SurveyUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для ответа на опрос.
    /// </summary>
    [JsonPropertyName("surveyAnswerUrl")]
    public string SurveyAnswerUrl => CalculateUrl(_endpointsConfig.SurveyAnswerUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для списка опросов.
    /// </summary>
    [JsonPropertyName("surveyListUrl")]
    public string SurveyListUrl => CalculateUrl(_endpointsConfig.SurveyListUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для отката опроса.
    /// </summary>
    [JsonPropertyName("surveyRollbackUrl")]
    public string SurveyRollbackUrl => CalculateUrl(_endpointsConfig.SurveyRollbackUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для оценки опроса.
    /// </summary>
    [JsonPropertyName("surveyRateUrl")]
    public string SurveyRateUrl => CalculateUrl(_endpointsConfig.SurveyRateUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для беременности.
    /// </summary>
    [JsonPropertyName("pregnancyUrl")]
    public string PregnancyUrl => CalculateUrl(_endpointsConfig.PregnancyUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для детей.
    /// </summary>
    [JsonPropertyName("childrenUrl")]
    public string ChildrenUrl => CalculateUrl(_endpointsConfig.ChildrenUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL для платежной службы.
    /// </summary>
    [JsonPropertyName("paymentServiceUrl")]
    public string PaymentServiceUrl => CalculateUrl(_endpointsConfig.PaymentServiceUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL для подписки.
    /// </summary>
    [JsonPropertyName("subscriptionUrl")]
    public string SubscriptionUrl => CalculateUrl(_endpointsConfig.SubscriptionUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL для оформления заказа через Robokassa.
    /// </summary>
    [JsonPropertyName("robokassaCheckoutUrl")]
    public string RobokassaCheckoutUrl => CalculateUrl(_endpointsConfig.RobokassaCheckoutUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL для промокода.
    /// </summary>
    [JsonPropertyName("promocodeUrl")]
    public string PromocodeUrl => CalculateUrl(_endpointsConfig.PromocodeUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL для расчета цены.
    /// </summary>
    [JsonPropertyName("calculatePriceUrl")]
    public string CalculatePriceUrl => CalculateUrl(_endpointsConfig.CalculatePriceUrl, _reverseProxyConfig.PaymentProxy);
}