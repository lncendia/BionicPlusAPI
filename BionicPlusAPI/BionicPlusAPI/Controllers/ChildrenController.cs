using BionicPlusAPI.Data.Interfaces;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Children;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace BionicPlusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChildrenController : Controller
    {
        private readonly IChildrenService _childrenService;

        public ChildrenController(IChildrenService childrenService)
        {
            _childrenService = childrenService;
        }


        [HttpGet("", Name = "GetChildren")]
        public async Task<ActionResult<List<Child>>> GetChildren([Required, FromQuery] bool isActive)
        {
            var children = await _childrenService.GetChildren(isActive);

            return children;
        }

        [HttpPost("", Name = "AddChild")]
        public async Task<ActionResult> AddChild([FromBody] Child child)
        {
            var id = await _childrenService.AddChild(child);

            return Ok(new { childId = id});
        }

        [HttpDelete("archive", Name = "ArchiveChild")]
        public async Task<ActionResult> ArchiveChild([FromQuery] string id)
        {
            await _childrenService.ArchiveChild(id);

            return Ok();
        }

        [HttpDelete("", Name = "DeleteChild")]
        public async Task<ActionResult> DeleteChild([FromQuery] string id)
        {
            await _childrenService.DeleteChild(id);

            return Ok();
        }

        [HttpPost("measurement", Name = "AddMeasurement")]
        public async Task<ActionResult> AddMeasurement([FromQuery] string childId, [FromBody] ChildMeasurement measurement)
        {
            await _childrenService.AddMeasurement(childId, measurement);

            return Ok();
        }

        [HttpPut("measurement", Name = "UpdateMeasurement")]
        public async Task<ActionResult> UpdateMeasurement([FromQuery] string childId, [FromQuery] DateTime date, [FromBody] ChildMeasurement measurement)
        {
            var dateOnly = new DateOnly(date.Year, date.Month, date.Day);

            await _childrenService.UpdateMeasurement(childId, dateOnly, measurement);

            return Ok();
        }

        [HttpDelete("measurement", Name = "DeleteMeasurement")]
        public async Task<ActionResult> DeleteMeasurement([FromQuery] string childId, [FromQuery] DateTime date)
        {
            var dateOnly = new DateOnly(date.Year, date.Month, date.Day);

            await _childrenService.DeleteMeasurement(childId, dateOnly);

            return Ok();
        }
    }
}
