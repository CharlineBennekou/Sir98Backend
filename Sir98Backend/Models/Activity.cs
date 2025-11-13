namespace Sir98Backend.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public bool Cancelled { get; set; }

        public List<Instructor>? Instructors { get; set; }
    }
}
