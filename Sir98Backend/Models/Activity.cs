namespace Sir98Backend.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public DateTimeOffset StartUtc { get; set; } //Important to store in UTC, we convert to local time later on frontend


        public DateTimeOffset EndUtc { get; set; }

        public required string Address { get; set; }

        public bool Cancelled { get; set; }

        public string? Image { get; set; }

        public string? Link { get; set; }

        public string? Description { get; set; }

        public string? Tag { get; set; }

        // many-to-many with Instructor
        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

        //If the event is recurring, we will use a format called Rrule to save the pattern as a string which the frontend will use to generate the recurrences.
        public bool IsRecurring { get; set; }
        public string? Rrule { get; set; }

        // Many activities can have many subscriptions
        public ICollection<ActivitySubscription> ActivitySubscriptions { get; set; } = new List<ActivitySubscription>();

        // Changes / history of this activity
        public ICollection<ChangedActivity> ChangedActivities { get; set; } = new List<ChangedActivity>();




    }
}
