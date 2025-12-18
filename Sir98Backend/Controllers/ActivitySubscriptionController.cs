using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<ActivitySubscription>>> GetAll()
        {
            var subscriptions = await _repository.GetAllAsync();
            return Ok(subscriptions);
        }

        // GET: api/ActivitySubscription/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ActivitySubscription>>> GetByUserId(string userId)
        {
            var result = await _repository.GetByUserIdAsync(userId);
            return Ok(result);
        }

        // POST: api/ActivitySubscription
        [HttpPost]
        public async Task<ActionResult<ActivitySubscription>> Post([FromBody] ActivitySubscriptionPostDto subscription)
        {
            if (subscription == null)
                return BadRequest("Body is required.");

            if (string.IsNullOrWhiteSpace(subscription.UserId))
                return BadRequest("UserId is required.");

            if (subscription.ActivityId <= 0)
                return BadRequest("ActivityId must be greater than 0.");

            if (!subscription.AllOccurrences && subscription.OriginalStartUtc == default)
                return BadRequest("OriginalStartUtc is required for non-all-occurrence subscriptions.");

            try
            {
                var created = await _repository.AddAsync(subscription);

                if (created == null)
                    return Conflict("Already subscribed.");

                return Created($"api/ActivitySubscription/{created.Id}", created);
            }
            catch (DbUpdateException)
            {
                // real DB issue (FK/constraint/db down/etc) -> not "Already subscribed"
                return StatusCode(500, "Database error while creating subscription.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Unexpected error while creating subscription.");
            }
        }


        [HttpDelete]
        public async Task<IActionResult> UnsubscribeAsync([FromBody] ActivitySubscriptionDeleteDto subscription)
        {
            if (subscription == null)
                return BadRequest("Body is required.");

            if (string.IsNullOrWhiteSpace(subscription.UserId))
                return BadRequest("UserId is required.");

            bool deleted = await _repository.DeleteAsync(subscription);


            if (!deleted)
                return NotFound("Subscription not found.");

            return NoContent(); // 204
        }
    }
}
