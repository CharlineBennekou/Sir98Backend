using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Ocsp;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Repository;
using Sir98Backend.Services;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;

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
            [FromQuery] int days = 7, //Defaults to 7 days if not provided
            [FromQuery] string? filter = null,
            [FromQuery] string? userId = null,
            [FromQuery] int? activityId = null)
        {
            if (days <= 0)
            
                return BadRequest("You must go at least 1 day forward");
            

            if (from.HasValue && from.Value.Offset != TimeSpan.Zero)
            
                return BadRequest("The 'from' parameter must be sent in UTC (offset +00:00). Example: 2026-03-03T17:00:00Z");
            

            if (filter == "mine" && userId==null) //If you aren't logged in, you cant filter by mine. Frontend should prevent this, but just in case, we will clear filter
            
                filter = null;
            

            var fromUtc = (from ?? DateTimeOffset.UtcNow).ToUniversalTime();

            var occurrences = _service.GetOccurrences(fromUtc, days, filter, userId).ToList();

            // Hvis userId er angivet, marker occurrences baseret på brugerens subscriptions
            //if (!string.IsNullOrWhiteSpace(userId))
            //{
            //    var subs = _subRepo.GetByUserId(userId).ToList();

            //    foreach (var occ in occurrences)
            //    {
            //        occ.IsSubscribed = subs.Any(sub =>
            //            (sub.AllOccurrences && sub.ActivityId == occ.ActivityId) ||
            //            (!sub.AllOccurrences && sub.ActivityId == occ.ActivityId &&
            //             sub.OriginalStartUtc.HasValue && occ.OriginalStartUtc.HasValue &&
            //             sub.OriginalStartUtc.Value == occ.OriginalStartUtc.Value)
            //        );
            //    }
            //}

            if (activityId.HasValue)
            {
                occurrences = occurrences.Where(o => o.ActivityId == activityId.Value).ToList();
            }

            return Ok(occurrences);
        }


        //[HttpPost("subscribe")]
        //public ActionResult Subscribe([FromBody] SubscribeRequestDto req)
        //{
        //    if (req == null || string.IsNullOrWhiteSpace(req.UserId) || req.ActivityId <= 0)
        //        return BadRequest("userId and activityId are required.");

        //    var subscription = new ActivitySubscription
        //    {
        //        UserId = req.UserId,
        //        ActivityId = req.ActivityId,
        //        OriginalStartUtc = req.OriginalStartUtc,
        //        AllOccurrences = req.AllOccurrences
        //    };

        //    Console.WriteLine($"Subscribe called: user={subscription.UserId}, act={subscription.ActivityId}, original={subscription.OriginalStartUtc}, all={subscription.AllOccurrences}");

        //    var created = _subRepo.Add(subscription);
        //    if (created == null)
        //    {
        //        Console.WriteLine("Add returned null (duplicate or blocked).");
        //        // duplicate eller kunne ikke oprettes
        //        return Conflict(new { message = "Subscription already exists or could not be created." });
        //    }

        //    Console.WriteLine($"Created subscription id={created.Id}");
        //    // Returner Created med det nye objekt
        //    return Created($"/api/ActivitySubscription/{created.Id}", created);
        //}

        //[HttpPost("unsubscribe")]
        //public IActionResult Unsubscribe([FromBody] SubscribeRequestDto req)
        //{
        //    if (req == null || string.IsNullOrWhiteSpace(req.UserId) || req.ActivityId <= 0)
        //        return BadRequest("userId and activityId are required.");

        //    // Hvis originalStartUtc er null => slet series-subscription, ellers slet single
        //    var deleted = _subRepo.Delete(req.UserId, req.ActivityId, req.OriginalStartUtc);
        //    if (!deleted) return NotFound("Subscription not found.");
        //    return NoContent();

        //}


        ////var nowUtc = DateTimeOffset.UtcNow;
        ////var occurrences = _service.GetOccurrences(nowUtc, days, null, null).ToList();

        ////var subs = _subRepo.GetByUserId(userId).ToList();

        //foreach (var occ in occurrences)
        //{
        //    occ.IsSubscribed = subs.Any(subcription =>
        //    (subcription.AllOccurrences && subcription.ActivityId == occ.ActivityId) ||
        //    (!subcription.AllOccurrences && 
        //    subcription.ActivityId == occ.ActivityId 
        //    && subcription.OriginalStartUtc == occ.OriginalStartUtc)
        //    );
        //}

        //return Ok(occurrences);
    }



    
}
   

