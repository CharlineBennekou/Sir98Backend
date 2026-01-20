namespace Sir98Backend.Models.DataTransferObjects
{
    public class EditOccurrenceDto
    {
        public int Id { get; set; }
        public DateTimeOffset OriginalStartUtc { get; set; }

        public DateTimeOffset StartUtc { get; set; }
        public DateTimeOffset EndUtc { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }

        public bool IsCancelled { get; set; }

        public List<int> InstructorIds { get; set; } = new();
        public string? Tag { get; set; }
    }
}
