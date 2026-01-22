using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models.DataTransferObjects;
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
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Returns all activity occurrences in the [fromUtc, fromUtc+days) window.
        /// Example: GET /api/activity-occurrences?from=2025-12-01T00:00:00Z&days=7
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ActivityOccurrenceDto>>> Get(
         [FromQuery] DateTimeOffset? from = null, // Defaults to now if not provided
         [FromQuery] int days = 7,                // Defaults to 7 days if not provided
         [FromQuery] string? filter = null,
         [FromQuery] string? userId = null)
        {
            if (days <= 0)
            {
                return BadRequest("You must go at least 1 day forward");
            }
            if (from.HasValue && from.Value.Offset != TimeSpan.Zero)
            {
                return BadRequest("The 'from' parameter must be sent in UTC (offset +00:00). Example: 2026-03-03T17:00:00Z");
            }
            //If you aren't logged in, you cant filter by mine. Frontend should prevent this, but just in case, we will clear filter
            if (filter == "mine" && userId == null)
            {
                filter = null;
            }
            var fromUtc = (from ?? DateTimeOffset.UtcNow).ToUniversalTime();
            IEnumerable<ActivityOccurrenceDto> occurrences = new List<ActivityOccurrenceDto>();
            try
            {
                occurrences = await _service.GetOccurrencesAsync(fromUtc, days, filter, userId);
            }
            catch (Exception e)
            {
                Console.WriteLine("SKETE EN FEJL HER");
                Console.WriteLine(e);
            }
            return Ok(occurrences);
        }
    }
}
