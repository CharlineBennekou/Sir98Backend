using Sir98Backend.Models;

namespace Sir98Backend.Interface
{
    public interface IActivitySubscriptionRepo
    {
        IEnumerable<ActivitySubscription> GetAll();
        IEnumerable<ActivitySubscription> GetByUserId(string userId);
        ActivitySubscription? Add(ActivitySubscription subscription);
        bool Delete(string userId, int activityId, DateTimeOffset? originalStartUtc);

    }
}
