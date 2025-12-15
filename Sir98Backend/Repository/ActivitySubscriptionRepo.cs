using Microsoft.AspNetCore.SignalR;
using Sir98Backend.Models;
using Sir98Backend.Repository.Interface;

namespace Sir98Backend.Repository
{
    public class ActivitySubscriptionRepo : IActivitySubscriptionRepo
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

        public ActivitySubscription? Add(ActivitySubscription subscription)
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

                  subscription.OriginalStartUtc = null;
            }
            else
            {
                // Check duplicate single-subscription
                var exists = _subscriptions.Any(s =>
                s.UserId == subscription.UserId &&
                    s.ActivityId == subscription.ActivityId &&
                    s.OriginalStartUtc == subscription.OriginalStartUtc);

                if (exists) return null;
            }

            subscription.Id = _nextId++;
            _subscriptions.Add(subscription);
            Console.WriteLine("Added subscription; total now = " + _subscriptions.Count);

            return subscription;
        }

        public bool Delete(string userId, int activityId, DateTimeOffset? originalStartUtc)
        {
            ActivitySubscription? subscription;

            if (originalStartUtc == null)
            {
                subscription = _subscriptions.FirstOrDefault(s =>
                s.UserId == userId &&
                s.ActivityId == activityId &&
                s.AllOccurrences == true);
            }
            else
            {
                subscription = _subscriptions.FirstOrDefault(s =>
                    s.UserId == userId &&
                    s.ActivityId == activityId &&
                    s.OriginalStartUtc == originalStartUtc);
            }
            if (subscription == null)
                return false;

            _subscriptions.Remove(subscription);
            return true;

            
        }

    }
}

