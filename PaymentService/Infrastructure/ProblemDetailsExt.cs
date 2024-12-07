using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Infrastructure
{
    public class ProblemDetailsExt: ProblemDetails
    {
        public Dictionary<string, ICollection<string>>? Errors { get; set; }
    }
}
