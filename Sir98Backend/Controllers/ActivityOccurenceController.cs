using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Dtos;
using Sir98Backend.Services;

namespace Sir98Backend.Controllers
{
    [ApiController]
    [Route("api/activity-occurrences")]
    public class ActivityOccurrencesController : ControllerBase
    {
        private readonly ActivityOccurrenceService _service;

        public ActivityOccurrencesController(ActivityOccurrenceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Returns all activity occurrences in the [fromUtc, fromUtc+days) window.
        /// Example: GET /api/activity-occurrences?from=2025-12-01T00:00:00Z&days=7
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<ActivityOccurrenceDto>> Get(
            [FromQuery] DateTimeOffset? from = null, //Defaults to now if not provided
            [FromQuery] int days = 7) //Defaults to 7 days if not provided
        {
            if (days <= 0) days = 7; //Cant be less than 0

            if (from.HasValue && from.Value.Offset != TimeSpan.Zero)
            {
                return BadRequest("The 'from' parameter must be sent in UTC (offset +00:00). Example: 2026-03-03T17:00:00Z");
            }

            var fromUtc = (from ?? DateTimeOffset.UtcNow).ToUniversalTime();

            var occurrences = _service.GetOccurrences(fromUtc, days);

            return Ok(occurrences);
        }
    }
}
