using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Infrastracture
{
    public class ProblemDetailsExt: ProblemDetails
    {
        public Dictionary<string, ICollection<string>>? Errors { get; set; }
    }
}
