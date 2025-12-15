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
