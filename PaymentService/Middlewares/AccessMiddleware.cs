using Microsoft.Extensions.Options;
using PaymentService.Models;
using System.Net;

namespace PaymentService.Middlewares
{
    public class AccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccessMiddleware> _logger;
        private readonly List<string> _allowedIpAddresses;

        public AccessMiddleware(RequestDelegate next, IOptions<AllowedIps> allowedIps, ILogger<AccessMiddleware> logger)
        {
            _allowedIpAddresses = allowedIps.Value.Adresses.Split(",").ToList();
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
            _logger.LogError($"====> remote address: {remoteIpAddress}");
            await _next(context);
            //if (remoteIpAddress != null && _allowedIpAddresses.Contains(remoteIpAddress.ToString()))
            //{
            //    await _next(context);
            //}
            //else
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //}
        }
    }
}
