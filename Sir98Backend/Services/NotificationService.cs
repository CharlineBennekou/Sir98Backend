using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;

namespace Sir98Backend.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Notify everyone who has ANY subscription for this ActivityId
        /// (AllOccurrences = true OR false).
        /// Use when the entire activity/series is updated.
        /// </summary>
        public async Task NotifyUsersAboutSeriesChangeAsync(int activityId, string message)
        {
            var userIds = await GetUserIdsForSeriesChangeAsync(activityId);
            if (!userIds.Any()) return;

            var pushSubscriptions = await GetPushSubscriptionsForUserIdsAsync(userIds);
            if (!pushSubscriptions.Any()) return;

            await SendMessageToPushSubscriptionsAsync(pushSubscriptions, message);
        }

        /// <summary>
        /// Notify users subscribed to ALL occurrences OR the specific occurrence.
        /// Use when a ChangedActivity (single occurrence override) is updated.
        /// </summary>
        public async Task NotifyUsersAboutOccurrenceChangeAsync(
            int activityId,
            DateTimeOffset originalStartUtc,
            string message)
        {
            var userIds = await GetUserIdsForOccurrenceChangeAsync(activityId, originalStartUtc);
            if (!userIds.Any()) return;

            var pushSubscriptions = await GetPushSubscriptionsForUserIdsAsync(userIds);
            if (!pushSubscriptions.Any()) return;

            await SendMessageToPushSubscriptionsAsync(pushSubscriptions, message);
        }

        /// <summary>
        /// Helper method that gets all userIds subscribed to the given activityId.
        /// Used only on series-wide changes.
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        private async Task<List<string>> GetUserIdsForSeriesChangeAsync(int activityId)
        {
            return await _context.ActivitySubscriptions
                .AsNoTracking()
                .Where(s => s.ActivityId == activityId)
                .Select(s => s.UserId)
                .Distinct()
                .ToListAsync();
        }
        /// <summary>
        /// Helper method that gets all userIds subscribed to the given activityId.
        /// Used only on occurrence-specific changes.
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="originalStartUtc"></param>
        /// <returns></returns>
        private async Task<List<string>> GetUserIdsForOccurrenceChangeAsync(
            int activityId,
            DateTimeOffset originalStartUtc)
        {
            return await _context.ActivitySubscriptions
                .AsNoTracking()
                .Where(s =>
                    s.ActivityId == activityId &&
                    (s.AllOccurrences || s.OriginalStartUtc == originalStartUtc))
                .Select(s => s.UserId)
                .Distinct()
                .ToListAsync();
        }
        /// <summary>
        /// This method retrieves all PushSubscriptions for the given list of userIds.
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        private async Task<List<PushSubscription>> GetPushSubscriptionsForUserIdsAsync(
            List<string> userIds)
        {
            if (userIds.Count == 0)
                return new List<PushSubscription>();

            return await _context.PushSubscriptions
                .AsNoTracking()
                .Where(ps => userIds.Contains(ps.UserId))
                .ToListAsync();
        }

        private Task SendMessageToPushSubscriptionsAsync(
            List<PushSubscription> pushSubscriptions,
            string message)
        {
            throw new NotImplementedException("Push notification sending not implemented yet.");
        }
    }
}
