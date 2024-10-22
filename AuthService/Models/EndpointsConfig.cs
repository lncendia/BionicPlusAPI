using System.Text.Json.Serialization;

namespace AuthService.Models
{
    public class EndpointsConfig
    {
        [JsonPropertyName("checkAccountUrl")]
        public string CheckAccountUrl { get; set; } = string.Empty;

        [JsonPropertyName("loginUrl")]
        public string LoginUrl { get; set; } = string.Empty;

        [JsonPropertyName("registerUrl")]
        public string RegisterUrl { get; set; } = string.Empty;

        [JsonPropertyName("refreshTokensUrl")]
        public string RefreshTokensUrl { get; set; } = string.Empty;

        [JsonPropertyName("revokeTokensUrl")]
        public string RevokeTokensUrl { get; set; } = string.Empty;

        [JsonPropertyName("sendCodeUrl")]
        public string SendCodeUrl { get; set; } = string.Empty;

        [JsonPropertyName("confirmEmailUrl")]
        public string ConfirmEmailUrl { get; set; } = string.Empty;

        [JsonPropertyName("resendCodeUrl")]
        public string ResendCodeUrl { get; set; } = string.Empty;

        [JsonPropertyName("forgotPasswordUrl")]
        public string ForgotPasswordUrl { get; set; } = string.Empty;

        [JsonPropertyName("recoverPasswordUrl")]
        public string RecoverPasswordUrl { get; set; } = string.Empty;

        [JsonPropertyName("changePasswordUrl")]
        public string ChangePasswordUrl { get; set; } = string.Empty;

        [JsonPropertyName("cardListUrl")]
        public string CardListUrl { get; set; } = string.Empty;

        [JsonPropertyName("cardUrl")]
        public string CardUrl { get; set; } = string.Empty;

        [JsonPropertyName("basicCardUrl")]
        public string BasicCardUrl { get; set; } = string.Empty;

        [JsonPropertyName("imageUploadUrl")]
        public string ImageUploadURL { get; set; } = string.Empty;

        [JsonPropertyName("profileUrl")]
        public string ProfileUrl { get; set; } = string.Empty;

        [JsonPropertyName("profileUpdateUrl")]
        public string ProfileUpdateUrl { get; set; } = string.Empty;

        [JsonPropertyName("profileUnitsUrl")]
        public string ProfileUnitsUrl { get; set; } = string.Empty;

        [JsonPropertyName("profileMeasureUrl")]
        public string ProfileMeasureUrl { get; set; } = string.Empty;

        [JsonPropertyName("surveyUrl")]
        public string SurveyUrl { get; set; } = string.Empty;

        [JsonPropertyName("surveyAnswerUrl")]
        public string SurveyAnswerUrl { get; set; } = string.Empty;

        [JsonPropertyName("surveyListUrl")]
        public string SurveyListUrl { get; set; } = string.Empty;

        [JsonPropertyName("surveyRollbackUrl")]
        public string SurveyRollbackUrl { get; set; } = string.Empty;

        [JsonPropertyName("surveyRateUrl")]
        public string SurveyRateUrl { get; set; } = string.Empty;

        [JsonPropertyName("pregnancyUrl")]
        public string PregnancyUrl { get; set; } = string.Empty;

        [JsonPropertyName("childrenUrl")]
        public string ChildrenUrl { get; set; } = string.Empty;

        [JsonPropertyName("paymentServiceUrl")]
        public string PaymentServiceUrl { get; set; } = string.Empty;

        [JsonPropertyName("subscriptionUrl")]
        public string SubscriptionUrl { get; set; } = string.Empty;

        [JsonPropertyName("robokassaCheckoutUrl")]
        public string RobokassaCheckoutUrl { get; set; } = string.Empty;

        [JsonPropertyName("promocodeUrl")]
        public string PromocodeUrl { get; set; } = string.Empty;

        [JsonPropertyName("calculatePriceUrl")]
        public string CalculatePriceUrl { get; set; } = string.Empty;
    }
}
