using Sir98Backend.Models;

namespace Sir98Backend.Services
{
    public class ActivityNotificationPayloadBuilder
    {
        // Denmark time zone id differs by OS
        // Windows: "Romance Standard Time"
        // Linux (common in Azure): "Europe/Copenhagen"
        private static readonly TimeZoneInfo DanishTz = ResolveDanishTimeZone();

        public NotificationPayload BuildSeriesChange(Activity activity)
        {
            var local = TimeZoneInfo.ConvertTime(activity.StartUtc, DanishTz);

            // Example format: 26-03-2026 13:00
            var danishDateTime = local.ToString("dd-MM-yyyy HH:mm");

            var isCancelled = activity.Cancelled;

            var verbTitle = isCancelled ? "er blevet aflyst" : "er blevet ændret";
            var verbBody = isCancelled ? "er blevet aflyst" : "er blevet ændret";

            return new NotificationPayload
            {
                Title = $"{activity.Title} {verbTitle}.",
                Body = $"{activity.Title} den {danishDateTime} {verbBody}. Klik for at se mere.",
                Url = "http://localhost:5173/account-settings"
            };
        }

        private static TimeZoneInfo ResolveDanishTimeZone()
        {
            // Try common ids across Windows + Linux
            try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen"); }
            catch { /* ignore */ }

            try { return TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"); }
            catch { /* ignore */ }

            // Fallback: if not found, default to UTC to avoid crashing
            return TimeZoneInfo.Utc; //Todo: should probably log this somewhere.
        }
    }
}
