using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Interface;
using Sir98Backend.Models;

public class PushSubscriptionService : IPushSubscriptionService
{
    private readonly AppDbContext _context;

    public PushSubscriptionService(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    /// <summary>
    /// Insert or update a push subscription for the given user.
    /// </summary>
    /// <param name="userEmail"></param>
    /// <param name="endpoint"></param>
    /// <param name="p256dh"></param>
    /// <param name="auth"></param>
    /// <returns></returns>
    public async Task UpsertAsync(string userEmail, string endpoint, string p256dh, string auth)
    {
        var now = DateTimeOffset.UtcNow;

        var existing = await _context.PushSubscriptions
            .SingleOrDefaultAsync(ps => ps.Endpoint == endpoint);

        if (existing is null)
        {
            var entity = new PushSubscription
            {
                UserId = userEmail,
                Endpoint = endpoint,
                P256dh = p256dh,
                Auth = auth,
                CreatedAtUtc = now,
                LastUsedUtc = now
            };

            _context.PushSubscriptions.Add(entity);
        }
        else
        {
            // Endpoint exists already: update (also handles "endpoint moved to new user" cases safely)
            existing.UserId = userEmail;
            existing.P256dh = p256dh;
            existing.Auth = auth;
            existing.LastUsedUtc = now;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(string userEmail, string endpoint)
    {
        // Optional security choice:
        // Restrict deletion to subscriptions owned by the current user.
        var entity = await _context.PushSubscriptions
            .SingleOrDefaultAsync(ps => ps.Endpoint == endpoint && ps.UserId == userEmail);

        if (entity is null) return;

        _context.PushSubscriptions.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
