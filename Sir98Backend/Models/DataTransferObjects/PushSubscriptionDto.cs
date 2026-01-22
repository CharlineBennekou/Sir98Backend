using System.ComponentModel.DataAnnotations;

namespace Sir98Backend.Models.DataTransferObjects
{
    public class PushSubscriptionDto
    {
        [Required]
        public required string UserId { get; set; }   // Email (temporary)

        [Required]
        public required string Endpoint { get; set; }

        [Required]
        public required string P256dh { get; set; }

        [Required]
        public required string Auth { get; set; }
    }
}
