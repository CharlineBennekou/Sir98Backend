// Sir98Backend/Controllers/PushSubscriptionController.cs
using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Interfaces;
using Sir98Backend.Models.DataTransferObjects;

namespace Sir98Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PushSubscriptionController : ControllerBase
    {
        private readonly IPushSubscriptionService _service;

        public PushSubscriptionController(IPushSubscriptionService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] PushSubscriptionDto dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.UserId) ||
                string.IsNullOrWhiteSpace(dto.Endpoint) ||
                string.IsNullOrWhiteSpace(dto.P256dh) ||
                string.IsNullOrWhiteSpace(dto.Auth))
            {
                return BadRequest("Invalid subscription data.");
            }

            await _service.UpsertAsync(dto.UserId, dto.Endpoint, dto.P256dh, dto.Auth);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery] string userId, [FromQuery] string endpoint)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(endpoint))
                return BadRequest("userId and endpoint are required.");

            await _service.RemoveAsync(userId, endpoint);
            return NoContent();
        }

        // Dev/test endpoint (optional)
        [HttpPost("pushall")]
        public async Task<IActionResult> PushAll()
        {
            var result = await _service.PushAllAsync(
                title: "Test notifikation",
                body: "Modtaget push fra pushall(dev metode)",
                url: "http://localhost:5173/aktiviteter"
            );

            if (result.TotalAttempted == 0)
                return Ok(new { message = "No subscriptions to notify." });

            return Ok(new
            {
                message = "PushAll triggered.",
                totalAttempted = result.TotalAttempted,
                failed = result.Failed,
                removedExpired = result.RemovedExpired
            });
        }
    }
}
