namespace Sir98Backend.Models
{
    /// <summary>
    /// This is an instructor and represents the person that is leading an activity. In real life, they have other titles but we use instructor for simplicity. This is not to be confused with the User's role as instructor.
    /// </summary>
    public class Instructor
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public string FirstName { get; set; }
        public string Image { get; set; }
    }
}
