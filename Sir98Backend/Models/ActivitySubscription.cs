namespace Sir98Backend.Models
{
    /// <summary>
    /// This class represents a user's subscription to an activity. It connects a userId to the ActivityId they are subscribed to. AllOccurrences indicates if the user is subscribed to all occurrences of a recurring activity. If false, it is a subscription to the specific occurrence identified by OriginalStartUtc.
    /// </summary>
    public class ActivitySubscription
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int ActivityId { get; set; }
        public DateTimeOffset OriginalStartUtc { get; set; } //Normal activities can be identified by just the ActivityId, but recurring activities need the StartUtc to identify the specific occurrence. This *needs* to be the original start time of the occurrence.

        public bool AllOccurrences { get; set; }



        // Navigation property to the Activity
        public Activity? Activity { get; set; }

        //Navigation property to the User
        public User? User { get; set; }

    }
}
