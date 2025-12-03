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

            subscription.Id = _nextId++;
            _subscriptions.Add(subscription);

            return subscription;
        }

        public bool DeleteById(int id)
        {
            var existing = _subscriptions.FirstOrDefault(s => s.Id == id);
            if (existing == null)
                return false;

            _subscriptions.Remove(existing);
            return true;
        }
    }
}

