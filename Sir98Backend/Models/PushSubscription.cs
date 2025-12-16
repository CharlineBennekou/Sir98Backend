using System.ComponentModel.DataAnnotations;

namespace Sir98Backend.Models
{
    /// <summary>
    /// Represents a single browser/device push subscription for a user.
    /// A user may have multiple subscriptions (e.g. phone, laptop).
    /// </summary>
    public class PushSubscription
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Unique endpoint URL provided by the browser push service.
        /// This is the best unique identifier for a subscription.
        /// </summary>
        [Required]
        public string Endpoint { get; set; }

        [Required]
        public string P256dh { get; set; }

        [Required]
        public string Auth { get; set; }

        /// <summary>
        /// When this subscription was first created (stored).
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; set; }

        /// <summary>
        /// Last time this subscription was successfully used or refreshed.
        /// Used for cleanup of stale subscriptions.
        /// </summary>
        public DateTimeOffset LastUsedUtc { get; set; }
    }
}
