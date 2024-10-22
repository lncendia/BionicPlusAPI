using DomainObjects.Pregnancy.Localizations;
using DomainObjects.Pregnancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BionicPlusAPI.Data.Interfaces;

namespace BionicPlusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PregnancyController : Controller
    {
        private readonly IPregnancy _pregnancyService;

        public PregnancyController(IPregnancy pregnancyService)
        {
            _pregnancyService = pregnancyService;
        }

        [HttpPost("", Name = "AddPregnancy")]
        public async Task<ActionResult<Pregnancy>> AddPregnancy(Pregnancy pregnancy)
        {
            var addResult = await _pregnancyService.AddPregnancy(pregnancy);

            return addResult != null ? Ok(addResult) : BadRequest();
        }

        [HttpGet("", Name = "GetPregnancies")]
        public async Task<ActionResult<Pregnancy>> GetPregnancies([Required, FromQuery] bool isActive)
        {
            var addResult = await _pregnancyService.GetPregnancies(isActive);

            return addResult != null ? Ok(addResult) : BadRequest();
        }

        [HttpDelete("archive", Name = "ArchivePregnancy")]
        public async Task<ActionResult<string>> ArchivePregnancy(string id)
        {
            await _pregnancyService.ArchivePregnancy(id);

            return Ok();
        }

        [HttpDelete("", Name = "DeletePregnancy")]
        public async Task<ActionResult<string>> DeletePregnancy(string id)
        {
            await _pregnancyService.DeletePregnancy(id);

            return Ok();
        }


        [HttpPost("weighing", Name = "AddWeighing")]
        public async Task<ActionResult> AddWeighing([FromQuery] string id, [FromBody] Weighing weighing)
        {
            await _pregnancyService.AddWeighing(id, weighing);

            return Ok();
        }

        [HttpPut("weighing", Name = "ChangeWeighing")]
        public async Task<ActionResult<Pregnancy>> ChangeWeighing([FromQuery] string id, [FromBody] Weighing weighing)
        {
            await _pregnancyService.ChangeWeighing(id, weighing);

            return Ok();
        }

        [HttpDelete("weighing", Name = "DeleteWeighing")]
        public async Task<ActionResult<Pregnancy>> DeleteWeighing([FromQuery] string id, [FromQuery] DateTime date)
        {
            var dateOnly = new DateOnly(date.Year, date.Month, date.Day);

            await _pregnancyService.DeleteWeighing(id, dateOnly);

            return Ok();
        }

    }
}
