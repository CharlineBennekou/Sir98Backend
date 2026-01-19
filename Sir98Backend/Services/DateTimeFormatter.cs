using Sir98Backend.Interfaces;
using System.Globalization;

namespace Sir98Backend.Services
{
    public sealed class DateTimeFormatter : IDateTimeFormatter
    {
        private readonly TimeZoneInfo _danishTz;
        private readonly CultureInfo _culture;

        public DateTimeFormatter()
        {
            _danishTz = ResolveDanishTimeZone();
            _culture = CultureInfo.GetCultureInfo("da-DK");
        }

        public DateTimeOffset ToDanishLocal(DateTimeOffset utc)
        {
            return TimeZoneInfo.ConvertTime(utc, _danishTz);
        }

        public string DanishDateTime(DateTimeOffset utc)
        {
            var local = ToDanishLocal(utc);
            return local.ToString("dd-MM-yyyy HH:mm", _culture);
        }

        public string DanishDate(DateTimeOffset utc)
        {
            var local = ToDanishLocal(utc);
            return local.ToString("dd-MM-yyyy", _culture);
        }

        public string DanishTime(DateTimeOffset utc)
        {
            var local = ToDanishLocal(utc);
            return local.ToString("HH:mm", _culture);
        }

        // Denmark time zone id differs by OS
        // Windows: "Romance Standard Time"
        // Linux (common in Azure): "Europe/Copenhagen"
        private static TimeZoneInfo ResolveDanishTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen"); }
            catch { /* ignore */ }

            try { return TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"); }
            catch { /* ignore */ }

            return TimeZoneInfo.Utc; // TODO: log warning
        }
    }
}
