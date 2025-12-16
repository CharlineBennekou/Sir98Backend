using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sir98Backend.Data;
using Sir98Backend.Interface;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Services;
using WebPush;

namespace Sir98Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PushSubscriptionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPushSubscriptionService _pushSubscriptionService;
        private readonly VapidConfig _vapidConfig;

        public PushSubscriptionController(
            AppDbContext context,
            IPushSubscriptionService pushSubscriptionService,
            IOptions<VapidConfig> vapidOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pushSubscriptionService = pushSubscriptionService ?? throw new ArgumentNullException(nameof(pushSubscriptionService));
            _vapidConfig = vapidOptions?.Value ?? throw new ArgumentNullException(nameof(vapidOptions));
        }

        // Keeps your existing helper method
        [HttpGet("generate-vapid")]
        public IActionResult GenerateVapid()
        {
            var keys = VapidHelper.GenerateVapidKeys();
            return Ok(new
            {
                PublicKey = keys.PublicKey,
                PrivateKey = keys.PrivateKey
            });
        }

        /// <summary>
        /// TEST endpoint (kept): stores subscription using DbContext directly.
        /// This mirrors your old in-memory list but persists to DB now.
        /// </summary>
        [HttpPost("addtotest")]
        public async Task<IActionResult> AddToTest([FromBody] PushSubscriptionDto subscription)
        {
            if (subscription == null ||
                string.IsNullOrWhiteSpace(subscription.UserId) ||
                string.IsNullOrWhiteSpace(subscription.Endpoint) ||
                string.IsNullOrWhiteSpace(subscription.P256dh) ||
                string.IsNullOrWhiteSpace(subscription.Auth))
            {
                return BadRequest("Invalid subscription data.");
            }

            var now = DateTimeOffset.UtcNow;

            // Upsert-by-endpoint using DbContext (since you asked to keep context here)
            var existing = await _context.PushSubscriptions
                .SingleOrDefaultAsync(ps => ps.Endpoint == subscription.Endpoint);

            if (existing is null)
            {
                _context.PushSubscriptions.Add(new Models.PushSubscription
                {
                    UserId = subscription.UserId,
                    Endpoint = subscription.Endpoint,
                    P256dh = subscription.P256dh,
                    Auth = subscription.Auth,
                    CreatedAtUtc = now,
                    LastUsedUtc = now
                });
            }
            else
            {
                existing.UserId = subscription.UserId;
                existing.P256dh = subscription.P256dh;
                existing.Auth = subscription.Auth;
                existing.LastUsedUtc = now;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Subscription added to DB (test endpoint)." });
        }

        /// <summary>
        /// TEST endpoint (kept): sends a test push notification to all stored subscriptions.
        /// Also removes expired subscriptions (404/410) using DbContext directly.
        /// </summary>
        [HttpPost("pushall")]
        public async Task<IActionResult> PushAll()
        {
            var subscriptions = await _context.PushSubscriptions
                .AsNoTracking()
                .ToListAsync();

            if (subscriptions.Count == 0)
                return Ok(new { message = "No subscriptions to notify." });

            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails(
                _vapidConfig.Subject,
                _vapidConfig.PublicKey,
                _vapidConfig.PrivateKey);

            var payload = new
            {
                title = "Test notification",
                body = "Hello from SubscriptionController.PushAll 🎉",
                url = "/"
            };
            var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);

            var failed = 0;
            var expiredEndpoints = new List<string>();
            var touchedEndpoints = new List<string>();

            foreach (var sub in subscriptions)
            {
                var pushSubscription = new WebPush.PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);

                try
                {
                    webPushClient.SendNotification(pushSubscription, payloadJson, vapidDetails);
                    touchedEndpoints.Add(sub.Endpoint);
                }
                catch (WebPushException ex)
                {
                    failed++;

                    if (ex.StatusCode == System.Net.HttpStatusCode.Gone ||
                        ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        expiredEndpoints.Add(sub.Endpoint);
                    }
                }
            }

            // Cleanup expired subs (DbContext, per your request)
            if (expiredEndpoints.Count > 0)
            {
                var expired = await _context.PushSubscriptions
                    .Where(ps => expiredEndpoints.Contains(ps.Endpoint))
                    .ToListAsync();

                _context.PushSubscriptions.RemoveRange(expired);
            }

            // Update LastUsedUtc for successful sends (optional but aligns with your model)
            if (touchedEndpoints.Count > 0)
            {
                var now = DateTimeOffset.UtcNow;

                // Load & update tracked entities
                var touched = await _context.PushSubscriptions
                    .Where(ps => touchedEndpoints.Contains(ps.Endpoint))
                    .ToListAsync();

                foreach (var t in touched)
                    t.LastUsedUtc = now;
            }

            if (expiredEndpoints.Count > 0 || touchedEndpoints.Count > 0)
                await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "PushAll triggered.",
                totalAttempted = subscriptions.Count,
                failed,
                removedExpired = expiredEndpoints.Count
            });
        }

        // -------------------------
        // "REAL" endpoints using the SERVICE (kept separate from your test ones)
        // -------------------------

        /// <summary>
        /// Real endpoint: upsert using your service.
        /// Temporary: UserId is provided by frontend until auth is implemented.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] PushSubscriptionDto subscription)
        {
            if (subscription == null ||
                string.IsNullOrWhiteSpace(subscription.UserId) ||
                string.IsNullOrWhiteSpace(subscription.Endpoint) ||
                string.IsNullOrWhiteSpace(subscription.P256dh) ||
                string.IsNullOrWhiteSpace(subscription.Auth))
            {
                return BadRequest("Invalid subscription data.");
            }

            await _pushSubscriptionService.UpsertAsync(
                subscription.UserId,
                subscription.Endpoint,
                subscription.P256dh,
                subscription.Auth);

            return NoContent();
        }

        /// <summary>
        /// Real endpoint: remove using your service.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Remove([FromQuery] string userId, [FromQuery] string endpoint)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(endpoint))
                return BadRequest("userId and endpoint are required.");

            await _pushSubscriptionService.RemoveAsync(userId, endpoint);
            return NoContent();
        }
    }

}
