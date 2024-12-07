using DomainObjects.Infrastracture.Exceptions;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Options;

namespace PaymentService.Infrastructure
{
    public static class ProblemDetailsConfigurator
    {
        private static readonly Dictionary<int, string> RFC7231StatusMap = new()
        {
            [200] = "#section-6.3.1",
            [201] = "#section-6.3.2",
            [202] = "#section-6.3.3",
            [204] = "#section-6.3.5",
            [400] = "#section-6.5.1",
            [403] = "#section-6.5.3",
            [404] = "#section-6.5.4",
            [405] = "#section-6.5.5",
            [500] = "#section-6.6.1",
            [503] = "#section-6.6.4",
            [504] = "#section-6.6.5"
        };
        public static string GetRFC7231Link(int statusCode)
        {
            if (RFC7231StatusMap.TryGetValue(statusCode, out string chapter))
            {
                return $"https://tools.ietf.org/html/rfc7231{chapter}";
            }
            return null;
        }
        public static void Configure(this ProblemDetailsOptions options)
        {
            options.IncludeExceptionDetails = (ctx, ex) => false;

            options.Map<FormatException>(ex => new ProblemDetailsExt
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = GetRFC7231Link(StatusCodes.Status400BadRequest),
            });

            options.Map<ArgumentNullException>(ex => new ProblemDetailsExt
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = GetRFC7231Link(StatusCodes.Status400BadRequest),
            });

            options.Map<ArgumentException>(ex => new ProblemDetailsExt
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = GetRFC7231Link(StatusCodes.Status400BadRequest),
            });

            options.Map<InvalidOperationException>(ex => new ProblemDetailsExt
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = GetRFC7231Link(StatusCodes.Status400BadRequest),
            });

            options.Map<AccessViolationException>(ex => new ProblemDetailsExt
            {
                Title = ex.Message,
                Status = StatusCodes.Status403Forbidden,
                Type = GetRFC7231Link(StatusCodes.Status403Forbidden),
            });

            options.Map<HttpRequestException>(ex => new ProblemDetailsExt
            {
                Title = ex.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = GetRFC7231Link(StatusCodes.Status500InternalServerError),
            });

            options.Map<NotFoundException>(ex => new ProblemDetailsExt
            {
                Title = ex.Message,
                Status = StatusCodes.Status404NotFound,
                Type = GetRFC7231Link(StatusCodes.Status404NotFound),
            });


            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);


            options.Map<OptionsValidationException>(ex => new ProblemDetailsExt
            {
                Title = "Service misconfigurated",
                Status = StatusCodes.Status500InternalServerError,
                Type = GetRFC7231Link(StatusCodes.Status500InternalServerError),
            });

            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);

        }

        public static Action<ProblemDetailsOptions> Configure() => options => Configure(options);
    }
}

