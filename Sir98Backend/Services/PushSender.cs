// Sir98Backend/Services/Notifications/PushSender.cs
using Microsoft.Extensions.Options;
using Sir98Backend.Interfaces;
using Sir98Backend.Models;
using System.Net;
using WebPush;

namespace Sir98Backend.Services.Notifications
{
    public sealed class PushSender : IPushSender
    {
        private readonly VapidConfig _vapid;

        public PushSender(IOptions<VapidConfig> vapidOptions)
        {
            _vapid = vapidOptions?.Value ?? throw new ArgumentNullException(nameof(vapidOptions));
        }

        public Task<PushSendResult> SendAsync(IReadOnlyCollection<Models.PushSubscription> subscriptions, PushNotificationPayload payload)
        {
            if (subscriptions is null) throw new ArgumentNullException(nameof(subscriptions));
            if (payload is null) throw new ArgumentNullException(nameof(payload));

            if (subscriptions.Count == 0)
            {
                return Task.FromResult(new PushSendResult(
                    TotalAttempted: 0,
                    Succeeded: 0,
                    Failed: 0,
                    SucceededEndpoints: Array.Empty<string>(),
                    ExpiredEndpoints: Array.Empty<string>(),
                    Failures: Array.Empty<PushSendFailure>()));
            }

            var client = new WebPushClient();
            var vapidDetails = new VapidDetails(_vapid.Subject, _vapid.PublicKey, _vapid.PrivateKey);

            var payloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                title = payload.Title,
                body = payload.Body,
                url = payload.Url
            });

            var succeededEndpoints = new List<string>(capacity: subscriptions.Count);
            var expiredEndpoints = new List<string>();
            var failures = new List<PushSendFailure>();

            foreach (var sub in subscriptions)
            {
                // Defensive: skip malformed rows instead of throwing and stopping the batch
                if (string.IsNullOrWhiteSpace(sub.Endpoint) ||
                    string.IsNullOrWhiteSpace(sub.P256dh) ||
                    string.IsNullOrWhiteSpace(sub.Auth))
                {
                    failures.Add(new PushSendFailure(
                        Endpoint: sub.Endpoint ?? "(null)",
                        Reason: "Invalid subscription data in database (missing endpoint/keys).",
                        StatusCode: null));
                    continue;
                }

                var pushSub = new WebPush.PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);

                try
                {
                    // WebPushClient is synchronous; that's fine for a minimal sender.
                    // If you later need scale, we can introduce throttled parallelism.
                    client.SendNotification(pushSub, payloadJson, vapidDetails);
                    succeededEndpoints.Add(sub.Endpoint);
                }
                catch (WebPushException ex)
                {
                    var statusCode = (int?)ex.StatusCode;

                    // 404/410 means subscription is gone/expired -> safe to delete
                    if (ex.StatusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        expiredEndpoints.Add(sub.Endpoint);
                    }

                    failures.Add(new PushSendFailure(
                        Endpoint: sub.Endpoint,
                        Reason: ex.Message,
                        StatusCode: statusCode));
                }
                catch (Exception ex)
                {
                    failures.Add(new PushSendFailure(
                        Endpoint: sub.Endpoint,
                        Reason: ex.Message,
                        StatusCode: null));
                }
            }

            var result = new PushSendResult(
                TotalAttempted: subscriptions.Count,
                Succeeded: succeededEndpoints.Count,
                Failed: failures.Count,
                SucceededEndpoints: succeededEndpoints,
                ExpiredEndpoints: expiredEndpoints,
                Failures: failures);

            return Task.FromResult(result);
        }

    }

}
