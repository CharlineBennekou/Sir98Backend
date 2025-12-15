using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;

namespace Sir98Backend.Repository
{
    public class ActivitySubscriptionRepo
    {
        private readonly AppDbContext _context;

        public ActivitySubscriptionRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivitySubscription>> GetAllAsync()
        {
            return await _context.ActivitySubscriptions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivitySubscription>> GetByUserIdAsync(string userId)
        {
            return await _context.ActivitySubscriptions
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<ActivitySubscription?> AddAsync(ActivitySubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var alreadySubscribed = await _context.ActivitySubscriptions.AnyAsync(s =>
                s.UserId == subscription.UserId &&
                s.ActivityId == subscription.ActivityId &&
                s.OriginalStartUtc == subscription.OriginalStartUtc
            );

            if (alreadySubscribed)
                return null;

            _context.ActivitySubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }

        public async Task<bool> DeleteAsync(string userId, int activityId, DateTimeOffset originalStartUtc)
        {
            var existing = await _context.ActivitySubscriptions.FirstOrDefaultAsync(s =>
                s.UserId == userId &&
                s.ActivityId == activityId &&
                s.OriginalStartUtc == originalStartUtc
            );

            if (existing == null)
                return false;

            _context.ActivitySubscriptions.Remove(existing);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
