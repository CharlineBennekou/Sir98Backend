using System.ComponentModel.DataAnnotations;

namespace Sir98Backend.Models.DataTransferObjects
{
    public class PushSubscriptionDto
    {
        [Required]
        public string UserId { get; set; }   // Email (temporary)

        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string P256dh { get; set; }

        [Required]
        public string Auth { get; set; }
    }
}
