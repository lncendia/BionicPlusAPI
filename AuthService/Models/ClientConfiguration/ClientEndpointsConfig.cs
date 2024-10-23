using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

public class ClientEndpointsConfig
{
    private readonly EndpointsConfig _endpointsConfig;

    private readonly ReverseProxyConfig _reverseProxyConfig;

    public ClientEndpointsConfig(EndpointsConfig endpointsConfig, ReverseProxyConfig reverseProxyConfig)
    {
        _endpointsConfig = endpointsConfig;
        _reverseProxyConfig = reverseProxyConfig;
    }

    private static string CalculateUrl(string url, string proxy)
    {
        if (string.IsNullOrEmpty(proxy)) return url;

        if (string.IsNullOrEmpty(url)) return url;

        // Разбираем оригинальный URL
        var originalUri = new Uri(url);

        // Разбираем новый базовый URL
        var newBaseUri = new Uri(proxy);

        // Создаем новый URL с использованием пути оригинального URL и нового базового URL
        return newBaseUri.ToString().TrimEnd('/') + originalUri.PathAndQuery;
    }

    [JsonPropertyName("checkAccountUrl")]
    public string CheckAccountUrl => CalculateUrl(_endpointsConfig.CheckAccountUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("loginUrl")]
    public string LoginUrl => CalculateUrl(_endpointsConfig.LoginUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("registerUrl")]
    public string RegisterUrl => CalculateUrl(_endpointsConfig.RegisterUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("refreshTokensUrl")]
    public string RefreshTokensUrl => CalculateUrl(_endpointsConfig.RefreshTokensUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("revokeTokensUrl")]
    public string RevokeTokensUrl => CalculateUrl(_endpointsConfig.RevokeTokensUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("sendCodeUrl")]
    public string SendCodeUrl => CalculateUrl(_endpointsConfig.SendCodeUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("confirmEmailUrl")]
    public string ConfirmEmailUrl => CalculateUrl(_endpointsConfig.ConfirmEmailUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("resendCodeUrl")]
    public string ResendCodeUrl => CalculateUrl(_endpointsConfig.ResendCodeUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("forgotPasswordUrl")]
    public string ForgotPasswordUrl => CalculateUrl(_endpointsConfig.ForgotPasswordUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("recoverPasswordUrl")]
    public string RecoverPasswordUrl =>
        CalculateUrl(_endpointsConfig.RecoverPasswordUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("changePasswordUrl")]
    public string ChangePasswordUrl => CalculateUrl(_endpointsConfig.ChangePasswordUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("cardListUrl")]
    public string CardListUrl => CalculateUrl(_endpointsConfig.CardListUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("cardUrl")]
    public string CardUrl => CalculateUrl(_endpointsConfig.CardUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("basicCardUrl")]
    public string BasicCardUrl => CalculateUrl(_endpointsConfig.BasicCardUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("imageUploadUrl")]
    public string ImageUploadUrl => CalculateUrl(_endpointsConfig.ImageUploadUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("profileUrl")]
    public string ProfileUrl => CalculateUrl(_endpointsConfig.ProfileUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("profileUpdateUrl")]
    public string ProfileUpdateUrl => CalculateUrl(_endpointsConfig.ProfileUpdateUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("profileUnitsUrl")]
    public string ProfileUnitsUrl => CalculateUrl(_endpointsConfig.ProfileUnitsUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("profileMeasureUrl")]
    public string ProfileMeasureUrl => CalculateUrl(_endpointsConfig.ProfileMeasureUrl, _reverseProxyConfig.LoginProxy);

    [JsonPropertyName("surveyUrl")]
    public string SurveyUrl => CalculateUrl(_endpointsConfig.SurveyUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("surveyAnswerUrl")]
    public string SurveyAnswerUrl => CalculateUrl(_endpointsConfig.SurveyAnswerUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("surveyListUrl")]
    public string SurveyListUrl => CalculateUrl(_endpointsConfig.SurveyListUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("surveyRollbackUrl")]
    public string SurveyRollbackUrl => CalculateUrl(_endpointsConfig.SurveyRollbackUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("surveyRateUrl")]
    public string SurveyRateUrl => CalculateUrl(_endpointsConfig.SurveyRateUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("pregnancyUrl")]
    public string PregnancyUrl => CalculateUrl(_endpointsConfig.PregnancyUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("childrenUrl")]
    public string ChildrenUrl => CalculateUrl(_endpointsConfig.ChildrenUrl, _reverseProxyConfig.ApiProxy);

    [JsonPropertyName("paymentServiceUrl")]
    public string PaymentServiceUrl =>
        CalculateUrl(_endpointsConfig.PaymentServiceUrl, _reverseProxyConfig.PaymentProxy);

    [JsonPropertyName("subscriptionUrl")]
    public string SubscriptionUrl => CalculateUrl(_endpointsConfig.SubscriptionUrl, _reverseProxyConfig.PaymentProxy);

    [JsonPropertyName("robokassaCheckoutUrl")]
    public string RobokassaCheckoutUrl =>
        CalculateUrl(_endpointsConfig.RobokassaCheckoutUrl, _reverseProxyConfig.PaymentProxy);

    [JsonPropertyName("promocodeUrl")]
    public string PromocodeUrl => CalculateUrl(_endpointsConfig.PromocodeUrl, _reverseProxyConfig.PaymentProxy);

    [JsonPropertyName("calculatePriceUrl")]
    public string CalculatePriceUrl =>
        CalculateUrl(_endpointsConfig.CalculatePriceUrl, _reverseProxyConfig.PaymentProxy);
}