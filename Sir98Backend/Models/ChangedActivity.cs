namespace Sir98Backend.Models
{
    /// <summary>
    /// This class is used to save exceptions to a recurring activity.
    /// They are uniquely identified by combining the Id of the parent activity with the originalStartUtc.
    /// All of the properties you can change is nullable so you only need to write what you want to change, rest is copied from the parent.
    /// </summary>
    public class ChangedActivity
    {

        public int Id { get; set; }

        public int ActivityId { get; set; }

        public DateTimeOffset OriginalStartUtc { get; set; } //It is important to preserve the originalStart since that is how we identify the activity. Dont change this property if you want a new start.

        public bool IsCancelled { get; set; }
        public DateTimeOffset? NewStartUtc { get; set; }
        public DateTimeOffset? NewEndUtc { get; set; }


        public string? NewTitle { get; set; }

        public string? NewDescription { get; set; }

        public string? NewAddress { get; set; }

        public ICollection<Instructor> NewInstructors { get; set; } = new List<Instructor>();

        public string? NewTag { get; set; }

        public required Activity Activity { get; set; }




    }
}
