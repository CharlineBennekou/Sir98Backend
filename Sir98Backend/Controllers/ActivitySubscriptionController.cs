using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Repository;
namespace Sir98Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitySubscriptionController : ControllerBase
    {
        private readonly ActivitySubscriptionRepo _repository;

        public ActivitySubscriptionController(ActivitySubscriptionRepo repository)
        {
            _repository = repository;
        }
        // GET: api/ActivitySubscription
        [HttpGet]
        public ActionResult<IEnumerable<ActivitySubscription>> GetAll()
        {
            var subscriptions = _repository.GetAll();
            return Ok(subscriptions);
        }

        // GET: api/ActivitySubscription/user/{userId}
        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<ActivitySubscription>> GetByUserId(string userId)
        {
            var result = _repository.GetByUserId(userId);
            // Still returning 200 OK with an empty list if none found
            return Ok(result);
        }

        // POST: api/ActivitySubscription
        [HttpPost]
        public ActionResult<ActivitySubscription> Post([FromBody] ActivitySubscription subscription)
        {
            if (subscription == null) return BadRequest("Body is required.");
            if (string.IsNullOrWhiteSpace(subscription.UserId)) return BadRequest("UserId is required.");
            if (subscription.ActivityId <= 0) return BadRequest("ActivityId must be > than 0.");

            var created = _repository.Add(subscription);
            if (created == null)
                return Conflict(new { message = "Subscription already exists or could not be created." });

            return Created($"api/ActivitySubscription/{created.Id}", created);



            /* try
             {
                 if (string.IsNullOrWhiteSpace(subscription.UserId))
                     return BadRequest("UserId is required.");

                 if (subscription.ActivityId <= 0)
                     return BadRequest("ActivityId must be greater than 0.");

                 var created = _repository.Add(subscription);
                 // Return 201 Created with the created entity
                 return Created($"api/ActivitySubscription/{created.Id}", created);
             }
             catch (Exception ex)
             {
                 if (subscription == null)
                 {
                     return BadRequest("Body is required.");
                 }

                 return StatusCode(500, $"Internal server error: {ex.Message}");
             }*/

        }

        [HttpDelete]
        public IActionResult Delete([FromBody] ActivitySubscription sub)
        {
            if (sub == null)
                return BadRequest("Body is required.");

            var deleted = _repository.Delete(sub.UserId, sub.ActivityId, sub.OriginalStartUtc);
            if (!deleted)
                return NotFound("Subscription not found.");
            return NoContent();

            /* var deleted = _repository.Delete(sub.UserId, sub.ActivityId, sub.OriginalStartUtc);

             if (!deleted)
                 return NotFound("Subscription not found.");

             // 204 No Content
             return NoContent();*/
        }

        [HttpPost("subscribe")]
        public ActionResult Subscribe([FromBody] SubscribeRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.UserId) || req.ActivityId <= 0)
                return BadRequest("userId and activityId are required.");

            var subscription = new ActivitySubscription
            {
                UserId = req.UserId,
                ActivityId = req.ActivityId,
                OriginalStartUtc = req.OriginalStartUtc,
                AllOccurrences = req.AllOccurrences
            };

            var created = _repository.Add(subscription);
            if (created == null)
                return Conflict(new { message = "Subscription already exists or could not be created." });

            return Created($"/api/ActivitySubscription/{created.Id}", created);
        }

        [HttpPost("unsubscribe")]
        public IActionResult Unsubscribe([FromBody] SubscribeRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.UserId) || req.ActivityId <= 0)
                return BadRequest("userId and activityId are required.");

            var deleted = _repository.Delete(req.UserId, req.ActivityId, req.OriginalStartUtc);
            if (!deleted) return NotFound("Subscription not found.");
            return NoContent();
        }


    }
}
