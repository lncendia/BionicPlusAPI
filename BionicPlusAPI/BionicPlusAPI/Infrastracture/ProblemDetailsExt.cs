using Microsoft.AspNetCore.Mvc;

namespace BionicPlusAPI.Infrastracture
{
    public class ProblemDetailsExt: ProblemDetails
    {
        public Dictionary<string, ICollection<string>>? Errors { get; set; }
    }
}
