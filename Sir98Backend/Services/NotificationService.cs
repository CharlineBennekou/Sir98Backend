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

        //Method that takes an activity, finds all subscription to the activity, and then returns a list of userIds to notify
        public async Task<List<string>> GetUserIdsToNotifyAsync(int activityId)
        {
            var userIds = await _context.ActivitySubscriptions
                .AsNoTracking()
                .Where(s => s.ActivityId == activityId)
                .Select(s => s.UserId)
                .Distinct()
                .ToListAsync();
            return userIds;
        }

        //Method that takes a list of userIds and then finds all PushSubscriptions for those users
        public async Task<List<PushSubscription>> GetPushSubscriptionsForUserIdsAsync(List<string> userIds)
        {
            var pushSubscriptions = await _context.PushSubscriptions
                .AsNoTracking()
                .Where(ps => userIds.Contains(ps.UserId))
                .ToListAsync();
            return pushSubscriptions;
        }

        //Method that takes a list of PushSubscriptions and a message, and then sends the message to all PushSubscriptions
        public async Task SendMessageToPushSubscriptionsAsync(List<PushSubscription> pushSubscriptions, string message)
        {
            throw new NotImplementedException("Push notification sending not implemented yet.");
        }

        //Method that combines the above methods to notify users about an activity
        public async Task NotifyUsersAboutActivityAsync(int activityId, string message)
        {
            var userIds = await GetUserIdsToNotifyAsync(activityId);
            var pushSubscriptions = await GetPushSubscriptionsForUserIdsAsync(userIds);
            await SendMessageToPushSubscriptionsAsync(pushSubscriptions, message);
        }
    }
}
