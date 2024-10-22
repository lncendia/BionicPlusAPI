using System.Net.Http.Headers;

namespace AuthService.Services.Providers
{
    public abstract class BaseRequestTokenProvider : DelegatingHandler
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly string _tokenName;

        public BaseRequestTokenProvider(string tokenName, IHttpContextAccessor httpContextAccessor) : base()
        {
            _tokenName = tokenName;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = RetrieveAccessToken();
                request.Headers.Authorization = new AuthenticationHeaderValue(_tokenName, token);
            }

            return base.Send(request, cancellationToken);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = RetrieveAccessToken();
                request.Headers.Authorization = new AuthenticationHeaderValue(_tokenName, token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        protected abstract string? RetrieveAccessToken();
    }
}
