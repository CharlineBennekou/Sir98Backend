using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;
using Sir98Backend.Repository.Interface;

namespace Sir98Backend.Repository
{
    public class ActivitySubscriptionRepo : IActivitySubscriptionRepo
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
            Console.WriteLine($"Repo.Add called: user={subscription.UserId}, act={subscription.ActivityId}, original={subscription.OriginalStartUtc}, all={subscription.AllOccurrences}");
            if (subscription.AllOccurrences)
            {
                // Check duplicate series-subscription
                var exists = _subscriptions.Any(s =>
                s.UserId == subscription.UserId &&
                    s.ActivityId == subscription.ActivityId &&
                    s.AllOccurrences == true);
                if (exists) return null;

            var alreadySubscribed = await _context.ActivitySubscriptions.AnyAsync(s =>
                s.UserId == subscription.UserId &&
                    s.ActivityId == subscription.ActivityId &&
                    s.OriginalStartUtc == subscription.OriginalStartUtc);

                if (exists) return null;
            }

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
