using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

/// <summary>
/// Configuration for client endpoints.
/// </summary>
public class ClientEndpointsConfig
{
    /// <summary>
    /// Configuration for endpoints.
    /// </summary>
    private readonly EndpointsConfig _endpointsConfig;

    /// <summary>
    /// Configuration for reverse proxy.
    /// </summary>
    private readonly ReverseProxyConfig _reverseProxyConfig;

    /// <summary>
    /// Constructor for the ClientEndpointsConfig class.
    /// </summary>
    /// <param name="endpointsConfig">Configuration for endpoints.</param>
    /// <param name="reverseProxyConfig">Configuration for reverse proxy.</param>
    public ClientEndpointsConfig(EndpointsConfig endpointsConfig, ReverseProxyConfig reverseProxyConfig)
    {
        _endpointsConfig = endpointsConfig;
        _reverseProxyConfig = reverseProxyConfig;
    }

    /// <summary>
    /// Calculates the URL using the proxy.
    /// </summary>
    /// <param name="url">Original URL.</param>
    /// <param name="proxy">Proxy URL.</param>
    /// <returns>New URL using the proxy.</returns>
    private static string CalculateUrl(string url, string proxy)
    {
        // If the proxy is empty, return the original URL
        if (string.IsNullOrEmpty(proxy)) return url;

        // If the original URL is empty, return it
        if (string.IsNullOrEmpty(url)) return url;

        // Parse the original URL
        var originalUri = new Uri(url);

        // Parse the new base URL
        var newBaseUri = new Uri(proxy);

        // Create a new URL using the path of the original URL and the new base URL
        return newBaseUri.ToString().TrimEnd('/') + originalUri.PathAndQuery;
    }

    /// <summary>
    /// URL for checking the account.
    /// </summary>
    [JsonPropertyName("checkAccountUrl")]
    public string CheckAccountUrl => CalculateUrl(_endpointsConfig.CheckAccountUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for login.
    /// </summary>
    [JsonPropertyName("loginUrl")]
    public string LoginUrl => CalculateUrl(_endpointsConfig.LoginUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for registration.
    /// </summary>
    [JsonPropertyName("registerUrl")]
    public string RegisterUrl => CalculateUrl(_endpointsConfig.RegisterUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for refreshing tokens.
    /// </summary>
    [JsonPropertyName("refreshTokensUrl")]
    public string RefreshTokensUrl => CalculateUrl(_endpointsConfig.RefreshTokensUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for revoking tokens.
    /// </summary>
    [JsonPropertyName("revokeTokensUrl")]
    public string RevokeTokensUrl => CalculateUrl(_endpointsConfig.RevokeTokensUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for confirming email.
    /// </summary>
    [JsonPropertyName("confirmEmailUrl")]
    public string ConfirmEmailUrl => CalculateUrl(_endpointsConfig.ConfirmEmailUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for resending code.
    /// </summary>
    [JsonPropertyName("resendCodeUrl")]
    public string ResendCodeUrl => CalculateUrl(_endpointsConfig.ResendCodeUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for forgot password.
    /// </summary>
    [JsonPropertyName("forgotPasswordUrl")]
    public string ForgotPasswordUrl => CalculateUrl(_endpointsConfig.ForgotPasswordUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for recovering password.
    /// </summary>
    [JsonPropertyName("recoverPasswordUrl")]
    public string RecoverPasswordUrl =>
        CalculateUrl(_endpointsConfig.RecoverPasswordUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for changing password.
    /// </summary>
    [JsonPropertyName("changePasswordUrl")]
    public string ChangePasswordUrl => CalculateUrl(_endpointsConfig.ChangePasswordUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for card list.
    /// </summary>
    [JsonPropertyName("cardListUrl")]
    public string CardListUrl => CalculateUrl(_endpointsConfig.CardListUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for card.
    /// </summary>
    [JsonPropertyName("cardUrl")]
    public string CardUrl => CalculateUrl(_endpointsConfig.CardUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for basic card.
    /// </summary>
    [JsonPropertyName("basicCardUrl")]
    public string BasicCardUrl => CalculateUrl(_endpointsConfig.BasicCardUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for image upload.
    /// </summary>
    [JsonPropertyName("imageUploadUrl")]
    public string ImageUploadUrl => CalculateUrl(_endpointsConfig.ImageUploadUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for profile.
    /// </summary>
    [JsonPropertyName("profileUrl")]
    public string ProfileUrl => CalculateUrl(_endpointsConfig.ProfileUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for updating profile.
    /// </summary>
    [JsonPropertyName("profileUpdateUrl")]
    public string ProfileUpdateUrl => CalculateUrl(_endpointsConfig.ProfileUpdateUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for profile units.
    /// </summary>
    [JsonPropertyName("profileUnitsUrl")]
    public string ProfileUnitsUrl => CalculateUrl(_endpointsConfig.ProfileUnitsUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for profile measurement.
    /// </summary>
    [JsonPropertyName("profileMeasureUrl")]
    public string ProfileMeasureUrl => CalculateUrl(_endpointsConfig.ProfileMeasureUrl, _reverseProxyConfig.LoginProxy);

    /// <summary>
    /// URL for survey.
    /// </summary>
    [JsonPropertyName("surveyUrl")]
    public string SurveyUrl => CalculateUrl(_endpointsConfig.SurveyUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for survey answer.
    /// </summary>
    [JsonPropertyName("surveyAnswerUrl")]
    public string SurveyAnswerUrl => CalculateUrl(_endpointsConfig.SurveyAnswerUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for survey list.
    /// </summary>
    [JsonPropertyName("surveyListUrl")]
    public string SurveyListUrl => CalculateUrl(_endpointsConfig.SurveyListUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for survey rollback.
    /// </summary>
    [JsonPropertyName("surveyRollbackUrl")]
    public string SurveyRollbackUrl => CalculateUrl(_endpointsConfig.SurveyRollbackUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for survey rating.
    /// </summary>
    [JsonPropertyName("surveyRateUrl")]
    public string SurveyRateUrl => CalculateUrl(_endpointsConfig.SurveyRateUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for pregnancy.
    /// </summary>
    [JsonPropertyName("pregnancyUrl")]
    public string PregnancyUrl => CalculateUrl(_endpointsConfig.PregnancyUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for children.
    /// </summary>
    [JsonPropertyName("childrenUrl")]
    public string ChildrenUrl => CalculateUrl(_endpointsConfig.ChildrenUrl, _reverseProxyConfig.ApiProxy);

    /// <summary>
    /// URL for payment service.
    /// </summary>
    [JsonPropertyName("paymentServiceUrl")]
    public string PaymentServiceUrl =>
        CalculateUrl(_endpointsConfig.PaymentServiceUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL for subscription.
    /// </summary>
    [JsonPropertyName("subscriptionUrl")]
    public string SubscriptionUrl => CalculateUrl(_endpointsConfig.SubscriptionUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL for Robokassa checkout.
    /// </summary>
    [JsonPropertyName("robokassaCheckoutUrl")]
    public string RobokassaCheckoutUrl =>
        CalculateUrl(_endpointsConfig.RobokassaCheckoutUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL for promocode.
    /// </summary>
    [JsonPropertyName("promocodeUrl")]
    public string PromocodeUrl => CalculateUrl(_endpointsConfig.PromocodeUrl, _reverseProxyConfig.PaymentProxy);

    /// <summary>
    /// URL for calculating price.
    /// </summary>
    [JsonPropertyName("calculatePriceUrl")]
    public string CalculatePriceUrl =>
        CalculateUrl(_endpointsConfig.CalculatePriceUrl, _reverseProxyConfig.PaymentProxy);
}