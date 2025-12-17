// Sir98Backend/Services/PushSubscriptionService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sir98Backend.Data;
using Sir98Backend.Interfaces;
using Sir98Backend.Models;
using WebPush;

namespace Sir98Backend.Services
{
    public class PushSubscriptionService : IPushSubscriptionService
    {
        private readonly AppDbContext _context;
        private readonly VapidConfig _vapid;

        public PushSubscriptionService(AppDbContext context, IOptions<VapidConfig> vapidOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _vapid = vapidOptions?.Value ?? throw new ArgumentNullException(nameof(vapidOptions));
        }

        public async Task UpsertAsync(string userEmail, string endpoint, string p256dh, string auth)
        {
            var now = DateTimeOffset.UtcNow;

            var existing = await _context.PushSubscriptions
                .SingleOrDefaultAsync(ps => ps.Endpoint == endpoint);

            if (existing is null)
            {
                _context.PushSubscriptions.Add(new Models.PushSubscription
                {
                    UserId = userEmail,
                    Endpoint = endpoint,
                    P256dh = p256dh,
                    Auth = auth,
                    CreatedAtUtc = now,
                    LastUsedUtc = now
                });
            }
            else
            {
                existing.UserId = userEmail;
                existing.P256dh = p256dh;
                existing.Auth = auth;
                existing.LastUsedUtc = now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(string userEmail, string endpoint)
        {
            var entity = await _context.PushSubscriptions
                .SingleOrDefaultAsync(ps => ps.Endpoint == endpoint && ps.UserId == userEmail);

            if (entity is null) return;

            _context.PushSubscriptions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PushAllResult> PushAllAsync(string title, string body, string url)
        {
            var subs = await _context.PushSubscriptions
                .AsNoTracking()
                .ToListAsync();

            if (subs.Count == 0)
                return new PushAllResult(0, 0, 0);

            var client = new WebPushClient();
            var vapidDetails = new VapidDetails(_vapid.Subject, _vapid.PublicKey, _vapid.PrivateKey);

            var payloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                title,
                body,
                url
            });

            var failed = 0;
            var expiredEndpoints = new HashSet<string>();
            var touchedEndpoints = new HashSet<string>();

            foreach (var s in subs)
            {
                var pushSub = new WebPush.PushSubscription(s.Endpoint, s.P256dh, s.Auth);

                try
                {
                    client.SendNotification(pushSub, payloadJson, vapidDetails);
                    touchedEndpoints.Add(s.Endpoint);
                }
                catch (WebPushException ex)
                {
                    failed++;
                    if (ex.StatusCode is System.Net.HttpStatusCode.Gone or System.Net.HttpStatusCode.NotFound)
                        expiredEndpoints.Add(s.Endpoint);
                }
            }

            if (expiredEndpoints.Count > 0)
            {
                var expired = await _context.PushSubscriptions
                    .Where(ps => expiredEndpoints.Contains(ps.Endpoint))
                    .ToListAsync();

                _context.PushSubscriptions.RemoveRange(expired);
            }

            if (touchedEndpoints.Count > 0)
            {
                var now = DateTimeOffset.UtcNow;
                var touched = await _context.PushSubscriptions
                    .Where(ps => touchedEndpoints.Contains(ps.Endpoint))
                    .ToListAsync();

                foreach (var t in touched)
                    t.LastUsedUtc = now;
            }

            if (expiredEndpoints.Count > 0 || touchedEndpoints.Count > 0)
                await _context.SaveChangesAsync();

            return new PushAllResult(subs.Count, failed, expiredEndpoints.Count);


        }
        /// <summary>
        /// Applies the result of a push send:
        /// - Removes expired subscriptions (404/410)
        /// - Updates LastUsedUtc for successful sends
        /// </summary>
        public async Task ApplySendResultAsync(PushSendResult result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            // Nothing to do
            if ((result.ExpiredEndpoints?.Count ?? 0) == 0 &&
                (result.SucceededEndpoints?.Count ?? 0) == 0)
            {
                return;
            }

            // Remove expired endpoints
            if (result.ExpiredEndpoints is { Count: > 0 })
            {
                var expired = await _context.PushSubscriptions
                    .Where(ps => result.ExpiredEndpoints.Contains(ps.Endpoint))
                    .ToListAsync(); // tracked

                if (expired.Count > 0)
                    _context.PushSubscriptions.RemoveRange(expired);
            }

            // Touch LastUsedUtc for succeeded endpoints
            if (result.SucceededEndpoints is { Count: > 0 })
            {
                var now = DateTimeOffset.UtcNow;

                var touched = await _context.PushSubscriptions
                    .Where(ps => result.SucceededEndpoints.Contains(ps.Endpoint))
                    .ToListAsync(); // tracked

                foreach (var t in touched)
                    t.LastUsedUtc = now;
            }

            await _context.SaveChangesAsync();
        }

    }
}
