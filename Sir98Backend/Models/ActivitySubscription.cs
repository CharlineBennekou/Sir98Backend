namespace Sir98Backend.Models
{
    /// <summary>
    /// This class represents a user's subscription to an activity. It connects a userId to the ActivityId they are subscribed to.
    /// </summary>
    public class ActivitySubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ActivityId { get; set; }
        public DateTimeOffset OriginalStartUtc { get; set; } //Normal activities can be identified by just the ActivityId, but recurring activities need the StartUtc to identify the specific occurrence. This *needs* to be the original start time of the occurrence.
        
        
    }
}
