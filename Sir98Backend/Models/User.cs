using System.ComponentModel.DataAnnotations;

namespace Sir98Backend.Models
{
    public class User
    {
        [Key]
        public required string Email { get; set; }

        public required string HashedPassword { get; set; }

        public required string Role { get; set; }

        public ICollection<ActivitySubscription> ActivitySubscriptions { get; set; } = new List<ActivitySubscription>();

    }
}
