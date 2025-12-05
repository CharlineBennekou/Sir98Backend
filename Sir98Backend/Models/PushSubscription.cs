namespace Sir98Backend.Models
{
    /// <summary>
    /// This class represents that a user has notifications enabled and contains the necessary information to send push notifications to them. They can have multiple subscriptions (e.g. different devices).
    /// </summary>
    public class PushSubscription
    {
        public string UserId { get; set; }
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }
}
