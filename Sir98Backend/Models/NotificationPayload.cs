namespace Sir98Backend.Models
{
    /// <summary>
    /// This is the payload that will be sent by PushSender when sending a push notification.
    /// </summary>
    public class NotificationPayload
    {
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
