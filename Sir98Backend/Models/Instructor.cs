using System.ComponentModel.DataAnnotations;
namespace Sir98Backend.Models
{
/// <summary>
/// This is an instructor and represents the person that is leading an activity. In real life, they have other titles but we use instructor for simplicity. This is not to be confused with the User's role as instructor.
/// </summary>
public class Instructor
    {
        [Key] // corrected casing for attribute
        public int Id { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Number { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(500)]
        public string Image { get; set; }

        // many instructors to many activities
        public ICollection<Activity> Activities { get; set; } =new List<Activity>();

        // Adding this just to follow the pattern, but I doubt we will use it..
        public ICollection<ChangedActivity> ChangedActivities { get; set; } = new List<ChangedActivity>();

    }
}
