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
        public async Task<ActionResult<ActivitySubscription>> Post([FromBody] ActivitySubscription subscription)
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

            if (subscription.ActivityId <= 0)
                return BadRequest("ActivityId must be greater than 0.");

            var created = await _repository.AddAsync(subscription);

            // If repo returns null when already subscribed:
            if (created == null)
                return Conflict("Already subscribed.");

            return Created($"api/ActivitySubscription/{created.Id}", created);
        }

        [HttpPost("unsubscribe")]
        public IActionResult Unsubscribe([FromBody] SubscribeRequestDto req)
        {
            if (sub == null)
                return BadRequest("Body is required.");

            bool deleted = await _repository.DeleteAsync(
                sub.UserId,
                sub.ActivityId,
                sub.OriginalStartUtc
            );

            if (!deleted)
                return NotFound("Subscription not found.");

            return NoContent(); // 204
        }
    }
}
