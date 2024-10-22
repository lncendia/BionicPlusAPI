using Microsoft.AspNetCore.Mvc;

namespace AuthService.Infrastracture
{
    public class ProblemDetailsExt: ProblemDetails
    {
        public Dictionary<string, ICollection<string>>? Errors { get; set; }
    }
}
