using Sir98Backend.Models;

namespace Sir98Backend.Models.DataTransferObjects
{
    public class ActivityOccurrenceDto
    {
        public int ActivityId { get; set; }

        /// <summary>
        /// The original start of the recurrence before any potential changes.
        /// Used to uniquely identify this occurrence when combined with the activityId
        /// This is not displayed in frontend.
        /// </summary>
        public DateTimeOffset? OriginalStartUtc { get; set; }

        /// <summary>
        /// The actual start time after applying any changes.
        /// For unchanged occurrences, this equals OriginalStartUtc.
        /// </summary>
        public DateTimeOffset StartUtc { get; set; }
        public DateTimeOffset EndUtc { get; set; }

        public string Title { get; set; } = "";
        public string Address { get; set; }
        public string? Image { get; set; } 
        public string? Link { get; set; } 
        public string? Description { get; set; }
        public List<Instructor>? Instructors { get; set; }

        public string? Tag { get; set; } = "";

        public bool Cancelled { get; set; }

        public bool IsSubscribed { get; set; }
    }

}
