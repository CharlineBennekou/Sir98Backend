using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
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
            try
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
            }
               
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] ActivitySubscription sub)
        {
            var deleted = _repository.Delete(sub.UserId, sub.ActivityId, sub.OriginalStartUtc);

            if (!deleted)
                return NotFound("Subscription not found.");

            // 204 No Content
            return NoContent();
        }

    }
}
