using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;

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

        public async Task<ActivitySubscription?> AddAsync(ActivitySubscriptionPostDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Duplicate check depends on subscription type
            var alreadySubscribed = dto.AllOccurrences
                ? await _context.ActivitySubscriptions.AnyAsync(s =>
                    s.UserId == dto.UserId &&
                    s.ActivityId == dto.ActivityId &&
                    s.AllOccurrences == true
                  )
                : await _context.ActivitySubscriptions.AnyAsync(s =>
                    s.UserId == dto.UserId &&
                    s.ActivityId == dto.ActivityId &&
                    s.AllOccurrences == false &&
                    s.OriginalStartUtc == dto.OriginalStartUtc
                  );

            if (alreadySubscribed)
                return null;

            // Map DTO -> Entity
            var entity = new ActivitySubscription
            {
                UserId = dto.UserId,
                ActivityId = dto.ActivityId,
                AllOccurrences = dto.AllOccurrences,
                OriginalStartUtc = dto.OriginalStartUtc
            };

            _context.ActivitySubscriptions.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }


        /// <summary>
        /// This method attempts first assumes the subscription to delete is for a single occurrence (AllOccurrences == false).
        /// If it cannot find such a subscription, it then attempts to delete an all-occurrences subscription (AllOccurrences == true).
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(ActivitySubscriptionDeleteDto subscription)
        {
            // 1) Try delete the specific occurrence but only if AllOccurrences is false
            if (subscription.OriginalStartUtc != null)
            {
                var single = await _context.ActivitySubscriptions.FirstOrDefaultAsync(s =>
                    s.UserId == subscription.UserId &&
                    s.ActivityId == subscription.ActivityId &&
                    s.AllOccurrences == false &&
                    s.OriginalStartUtc == subscription.OriginalStartUtc
                );

                if (single != null)
                {
                    _context.ActivitySubscriptions.Remove(single);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            // 2) If not found, we assume it's an all-occurrences subscription
            var all = await _context.ActivitySubscriptions.FirstOrDefaultAsync(s =>
                s.UserId == subscription.UserId &&
                s.ActivityId == subscription.ActivityId &&
                s.AllOccurrences == true
            );

            if (all == null)
                return false;

            _context.ActivitySubscriptions.Remove(all);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
