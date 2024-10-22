namespace AuthService.Services.Providers
{
    public class CurrentRequestBearerTokenProvider : BaseRequestTokenProvider
    {
        private const string AUTHORIZATION = "Authorization";
        private const string BEARER = "Bearer";

        public CurrentRequestBearerTokenProvider(IHttpContextAccessor httpContextAccessor)
            : base(BEARER, httpContextAccessor)
        {
        }

        protected override string? RetrieveAccessToken()
        {
            var httpRequest = _httpContextAccessor.HttpContext?.Request;

            if (httpRequest is not null && httpRequest.Headers.TryGetValue(AUTHORIZATION, out var authorizationHeader))
            {
                return authorizationHeader
                    .FirstOrDefault()?
                    .Replace(_tokenName + " ", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            }

            return null;
        }
    }
}
