using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Interfaces;
using Sir98Backend.Models;

namespace Sir98Backend.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;
        private readonly IPushSender _pushSender;
        private readonly IPushSubscriptionService _pushSubscriptionService;

        public NotificationService(AppDbContext context, IPushSender pushSender, IPushSubscriptionService pushSubscriptionService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pushSender = pushSender ?? throw new ArgumentNullException(nameof(pushSender));
            _pushSubscriptionService = pushSubscriptionService ?? throw new ArgumentNullException(nameof(pushSubscriptionService));

        }

        /// <summary>
        /// Notify everyone who has ANY subscription for this ActivityId
        /// (AllOccurrences = true OR false).
        /// Use when the entire activity/series is updated.
        /// </summary>
        public async Task NotifyUsersAboutSeriesChangeAsync(int activityId, NotificationPayload payload)
        {
            List<string> userIds = (await GetUserIdsForSeriesChangeAsync(activityId)).ToList();
            foreach (var email in userIds)
            {
                Console.WriteLine(email);
            }
            if (userIds.Count() == 0) return;

            List<PushSubscription> pushSubscriptions = await GetPushSubscriptionsForUserIdsAsync(userIds);
            if (pushSubscriptions.Count() == 0) return;

            await SendMessageToPushSubscriptionsAsync(pushSubscriptions, payload);
        }

        /// <summary>
        /// Notify users subscribed to ALL occurrences OR the specific occurrence.
        /// Use when a ChangedActivity (single occurrence override) is updated.
        /// </summary>
        public async Task NotifyUsersAboutOccurrenceChangeAsync(int activityId, DateTimeOffset originalStartUtc, NotificationPayload payload)
        {
            var userIds = await GetUserIdsForOccurrenceChangeAsync(activityId, originalStartUtc);
            if (!userIds.Any()) return;

            var pushSubscriptions = await GetPushSubscriptionsForUserIdsAsync(userIds);
            if (!pushSubscriptions.Any()) return;

            await SendMessageToPushSubscriptionsAsync(pushSubscriptions, payload);
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

            try
            {
                return await _context.PushSubscriptions
                .AsNoTracking()
                .Where(ps => userIds.Contains(ps.UserId))
                .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<PushSubscription>();
            }
        }
        /// <summary>
        /// Uses the PushSender to send the given payload to the list of PushSubscriptions. Then applies the result to the PushSubscriptionService to clean up invalid subscriptions.
        /// </summary>
        /// <param name="pushSubscriptions"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task SendMessageToPushSubscriptionsAsync(List<PushSubscription> pushSubscriptions, NotificationPayload payload)
        {
            var result = await _pushSender.SendAsync(
                subscriptions: pushSubscriptions, // List<T> works as IReadOnlyCollection<T>
                payload: new PushNotificationPayload(payload.Title, payload.Body, payload.Url));

            await _pushSubscriptionService.ApplySendResultAsync(result);
        }

    }
}
