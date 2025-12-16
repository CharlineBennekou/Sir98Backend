using System.ComponentModel.DataAnnotations;

namespace Sir98Backend.Models
{
    public class User
    {
        [Key]
        public string Email { get; set; }
       
        public string HashedPassword { get; set; }
        
        public string Role { get; set; }

        public ICollection<ActivitySubscription> ActivitySubscriptions { get; set; }

    }
}
