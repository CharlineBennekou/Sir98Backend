namespace Sir98Backend.Models.DataTransferObjects
{
    public class SubscribeRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public int ActivityId { get; set; }
        public DateTimeOffset? OriginalStartUtc { get; set; }
        public bool AllOccurrences { get; set; }
    }
}
