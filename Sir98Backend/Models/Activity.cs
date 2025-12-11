namespace Sir98Backend.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        public DateTimeOffset StartUtc { get; set; } //Important to store in UTC, we convert to local time later
        public DateTimeOffset EndUtc { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public List<Instructor>? Instructors { get; set; }
        public bool Cancelled { get; set; }
        public List<string>? Tags { get; set; }

        //If the event is recurring, we will use a format called Rrule to save the pattern as a string which the frontend will use to generate the recurrences.
        public bool IsRecurring { get; set; }
        public string? Rrule { get; set; }

        public ICollection<ActivitySubscription> ActivitySubscriptions { get; set; }
        = new List<ActivitySubscription>();
    }
}
