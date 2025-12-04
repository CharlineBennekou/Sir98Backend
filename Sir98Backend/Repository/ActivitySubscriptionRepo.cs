using Sir98Backend.Models;

namespace Sir98Backend.Repository
{
    public class ActivitySubscriptionRepo
    {
        private readonly List<ActivitySubscription> _subscriptions = new();
        private int _nextId = 1;

        public IEnumerable<ActivitySubscription> GetAll()
        {
            return _subscriptions;
        }

        public IEnumerable<ActivitySubscription> GetByUserId(string userId)
        {
            return _subscriptions
                .Where(s => s.UserId == userId)
                .ToList();
        }

        public ActivitySubscription Add(ActivitySubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            // Check if user is already subscribed to this activity at this time
            var alreadySubscribed = _subscriptions.Any(s =>
                s.UserId == subscription.UserId &&
                s.ActivityId == subscription.ActivityId &&
                s.OriginalStartUtc == subscription.OriginalStartUtc
            );

            subscription.Id = _nextId++;
            _subscriptions.Add(subscription);

            return subscription;
        }

        public bool Delete(string userId, int activityId, DateTimeOffset originalStartUtc)
        {
            var existing = _subscriptions.FirstOrDefault(s =>
                s.UserId == userId &&
                s.ActivityId == activityId &&
                s.OriginalStartUtc == originalStartUtc
            );

            if (existing == null)
                return false;

            _subscriptions.Remove(existing);
            return true;
        }

    }
}

