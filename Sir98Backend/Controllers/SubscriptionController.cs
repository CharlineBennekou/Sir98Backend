using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using WebPush;


namespace Sir98Backend.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class SubscriptionController : Controller
    {
        public static class TestSubscriptionStore
        {
            public static List<PushSubscriptionDto> Subscriptions { get; } = new();
        }

        // In a real app you’d inject this via DI / config
        private readonly VapidConfig _vapidConfig = new VapidConfig
        {
            Subject = "mailto:you@example.com",
            PublicKey = "BDVzVxg_Qd8OqCOHLmA4EAxxF_FQ8qAAv-jYmWSfxofkIWe69EZgJFl2lk-U18kbE6s-Jp9j7v-VrT8eEQDTarQ", //you may look here, but dont look below
            PrivateKey = "YxDoIeaf7SapX-Ye2qFOPpsNU9A0cxmmdz0vCtWvGxg" //dont look, avert your eyes
        };

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
        /// Frontend calls this once per device to store its push subscription.
        /// </summary>
        [HttpPost("addtotest")]
        public IActionResult AddToTest([FromBody] PushSubscriptionDto subscription)
        {
            if (subscription == null ||
                string.IsNullOrWhiteSpace(subscription.Endpoint) ||
                string.IsNullOrWhiteSpace(subscription.P256dh) ||
                string.IsNullOrWhiteSpace(subscription.Auth))
            {
                return BadRequest("Invalid subscription data.");
            }

            // For now: just add to in-memory list (no deduplication)
            TestSubscriptionStore.Subscriptions.Add(subscription);

            return Ok(new { message = "Subscription added to test list." });
        }

        /// <summary>
        /// Sends a test push notification to all stored test subscriptions.
        /// You can call this with a simple POST (no body needed).
        /// </summary>
        [HttpPost("pushall")]
        public IActionResult PushAll()
        {
            if (!TestSubscriptionStore.Subscriptions.Any())
            {
                return Ok(new { message = "No subscriptions to notify." });
            }

            var webPushClient = new WebPushClient();
            var vapidDetails = new VapidDetails(
                _vapidConfig.Subject,
                _vapidConfig.PublicKey,
                _vapidConfig.PrivateKey);

            // Simple message payload – what the service worker will receive.
            var payload = new
            {
                title = "Test notification",
                body = "Hello from SubscriptionController.PushAll 🎉",
                url = "/"
            };
            var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);

            var failed = new List<string>();

            foreach (var sub in TestSubscriptionStore.Subscriptions.ToList())
            {
                var pushSubscription = new WebPush.PushSubscription(
                    sub.Endpoint,
                    sub.P256dh,
                    sub.Auth);

                try
                {
                    webPushClient.SendNotification(pushSubscription, payloadJson, vapidDetails);
                }
                catch (WebPushException ex)
                {
                    // If subscription is invalid/expired, remove it
                    if (ex.StatusCode == System.Net.HttpStatusCode.Gone ||
                        ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TestSubscriptionStore.Subscriptions.Remove(sub);
                    }

                    failed.Add(sub.Endpoint);
                }
            }

            return Ok(new
            {
                message = "PushAll triggered.",
                total = TestSubscriptionStore.Subscriptions.Count,
                failed = failed.Count
            });
        }
    
}
}
