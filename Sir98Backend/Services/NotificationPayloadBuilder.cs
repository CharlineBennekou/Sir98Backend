using Sir98Backend.Interfaces;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;

namespace Sir98Backend.Services
{
    public class ActivityNotificationPayloadBuilder
    {
        private readonly IDateTimeFormatter _dateTime;

        public ActivityNotificationPayloadBuilder(IDateTimeFormatter dateTime)
        {
            _dateTime = dateTime;
        }

        public NotificationPayload BuildUpdatePayload(OccurrenceSnapshot before, OccurrenceSnapshot after, bool isSeries)
        {
            var title = BuildTitle(before, after, isSeries);
            var body = BuildBody(before, after);

            return new NotificationPayload
            {
                Title = title,
                Body = $"{after.Title}\n{body}",
                Url = "app.mnoergaard.dk/aktiviteter?type=mine"
            };
        }

        public string BuildTitle(OccurrenceSnapshot before, OccurrenceSnapshot after, bool isSeries)
        {
            var verbTitle = after.IsCancelled
                ? "er blevet aflyst"
                : "er blevet ændret";

            if (isSeries)
            {
                return $"Fremtidige {after.Title}-sessioner {verbTitle}.";
            }

            var danishDateTime = _dateTime.DanishDateTime(after.StartUtc);
            return $"{after.Title} den {danishDateTime} {verbTitle}.";
        }

        public string BuildBody(OccurrenceSnapshot before, OccurrenceSnapshot after)
        {
            var changes = new List<string>();
            //Title
            if (!string.Equals(before.Title, after.Title, StringComparison.Ordinal))
            {
                changes.Add($"Titel er ændret fra \"{before.Title}\" til \"{after.Title}\".");
            }

            //Description
            if (!string.Equals(before.Description, after.Description, StringComparison.Ordinal))
            {
                changes.Add($"Beskrivelse er til \"{after.Description ?? "-"}\".");
            }

            //Address
            if (!string.Equals(before.Address, after.Address, StringComparison.Ordinal))
            {
                changes.Add($"Adresse er ændret fra \"{before.Address ?? "-"}\" til \"{after.Address ?? "-"}\".");
            }

            //DateTime
            AddDateTimeChange(changes, "Start", before.StartUtc, after.StartUtc);
            AddDateTimeChange(changes, "Slut", before.EndUtc, after.EndUtc);

            //Instructors
            var beforeSet = before.InstructorIds?.ToHashSet() ?? new HashSet<int>();
            var afterSet = after.InstructorIds?.ToHashSet() ?? new HashSet<int>();

            if (!beforeSet.SetEquals(afterSet))
            {
                changes.Add("Instruktører er blevet ændret.");
            }

            if (changes.Count == 0)
                return "Der er ingen ændringer at vise.";

            // Join changes with new lines
            return string.Join("\n", changes);
        }

        /// <summary>
        /// Method used in BuildBody to add date/time changes to the changes list.
        /// </summary>
        /// <param name="changes"></param>
        /// <param name="label"></param>
        /// <param name="beforeUtc"></param>
        /// <param name="afterUtc"></param>
        private void AddDateTimeChange(List<string> changes, string label, DateTimeOffset beforeUtc, DateTimeOffset afterUtc)
        {
            var beforeLocal = _dateTime.ToDanishLocal(beforeUtc);
            var afterLocal = _dateTime.ToDanishLocal(afterUtc);

            var dateChanged = beforeLocal.Date != afterLocal.Date;
            var timeChanged = beforeLocal.TimeOfDay != afterLocal.TimeOfDay;

            if (!dateChanged && !timeChanged)
                return;

            if (dateChanged && timeChanged)
            {
                changes.Add(
                    $"{label} er rykket fra {_dateTime.DanishDateTime(beforeUtc)} " +
                    $"til {_dateTime.DanishDateTime(afterUtc)}."
                );
                return;
            }

            if (dateChanged)
            {
                changes.Add(
                    $"{label}dato er ændret fra {_dateTime.DanishDate(beforeUtc)} " +
                    $"til {_dateTime.DanishDate(afterUtc)}."
                );
                return;
            }

            // timeChanged only
            changes.Add(
                $"{label}tid er rykket fra {_dateTime.DanishTime(beforeUtc)} " +
                $"til {_dateTime.DanishTime(afterUtc)}."
            );
        }


    }
}
