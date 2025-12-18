namespace Sir98Backend.Models.DataTransferObjects
{
    public class ActivitySubscriptionDeleteDto
    {
        public string UserId { get; set; } = string.Empty;
        public int ActivityId { get; set; }
        public DateTimeOffset OriginalStartUtc { get; set; }
    }
}
