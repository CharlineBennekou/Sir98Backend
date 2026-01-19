namespace Sir98Backend.Models.DataTransferObjects
{
    public class OccurrenceSnapshot
    {

        public int ActivityId { get; set; }

        public DateTimeOffset? OriginalStartUtc { get; set; } //This is how we identify a recurrence in a series. It is nullable in case it isn't part of a series.

        public bool IsCancelled { get; set; }
        public DateTimeOffset StartUtc { get; set; }
        public DateTimeOffset EndUtc { get; set; }


        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        public List<int> InstructorIds { get; set; } = new();

        public string? Tag { get; set; }

        public OccurrenceSnapshot(int activityId, DateTimeOffset? originalStartUtc, bool isCancelled, DateTimeOffset startUtc, DateTimeOffset endUtc, string title, string? description, string? address, List<int> instructorIds, string? tag)
        {
            ActivityId = activityId;
            OriginalStartUtc = originalStartUtc;
            IsCancelled = isCancelled;
            StartUtc = startUtc;
            EndUtc = endUtc;
            Title = title;
            Description = description;
            Address = address;
            InstructorIds = instructorIds;
            Tag = tag;
        }
    }
}
