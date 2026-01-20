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
            var body = BuildBody(before, after, isSeries);

            return new NotificationPayload
            {
                Title = title,
                Body = body,
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

        public string BuildBody(OccurrenceSnapshot before, OccurrenceSnapshot after, bool isSeries)
        {
            var changes = new List<string>();

            if (after.IsCancelled)
            {
                changes.Add("Denne aktivitet er aflyst.");
                return string.Join("\n", changes);
            }
            //Title
            if (!string.Equals(before.Title, after.Title, StringComparison.Ordinal))
            {
                changes.Add($"Titel er ændret fra \"{before.Title}\" til \"{after.Title}\".");
            }

            //Description
            if (!string.Equals(before.Description, after.Description, StringComparison.Ordinal))
            {
                changes.Add($"Beskrivelse er ændret til \"{after.Description ?? "-"}\".");
            }

            //Address
            if (!string.Equals(before.Address, after.Address, StringComparison.Ordinal))
            {
                changes.Add($"Adresse er ændret fra \"{before.Address ?? "-"}\" til \"{after.Address ?? "-"}\".");
            }

            //DateTime
            AddDateTimeChange(changes, "Start", before.StartUtc, after.StartUtc, isSeries);
            AddDateTimeChange(changes, "Slut", before.EndUtc, after.EndUtc, isSeries);


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
        private void AddDateTimeChange(List<string> changes, string label, DateTimeOffset beforeUtc, DateTimeOffset afterUtc, bool isSeries)
        {
            var beforeLocal = _dateTime.ToDanishLocal(beforeUtc);
            var afterLocal = _dateTime.ToDanishLocal(afterUtc);

            var dateChanged = beforeLocal.Date != afterLocal.Date;
            var timeChanged = beforeLocal.TimeOfDay != afterLocal.TimeOfDay;

            if (!dateChanged && !timeChanged)
                return;

            // If it’s a series, show weekday instead of full date when "date" changed
            string BeforeDatePart() => isSeries ? _dateTime.DanishWeekday(beforeUtc) : _dateTime.DanishDate(beforeUtc);
            string AfterDatePart() => isSeries ? _dateTime.DanishWeekday(afterUtc) : _dateTime.DanishDate(afterUtc);

            // Combined line if both changed
            if (dateChanged && timeChanged)
            {
                if (isSeries)
                {
                    // Weekday + time is most meaningful for a series
                    changes.Add(
                        $"{label} er rykket fra {BeforeDatePart()} kl. {_dateTime.DanishTime(beforeUtc)} " +
                        $"til {AfterDatePart()} kl. {_dateTime.DanishTime(afterUtc)}."
                    );
                }
                else
                {
                    // Full datetime is best for one-off
                    changes.Add(
                        $"{label} er rykket fra {_dateTime.DanishDateTime(beforeUtc)} " +
                        $"til {_dateTime.DanishDateTime(afterUtc)}."
                    );
                }

                return;
            }

            // Date/weekday only
            if (dateChanged)
            {
                // Better Danish nouns than $"{label}dato"
                var dateLabel = label == "Slut" ? "Slutdag" : "Startdag";

                changes.Add($"{dateLabel} er ændret fra {BeforeDatePart()} til {AfterDatePart()}.");
                return;
            }

            // Time only
            var timeLabel = label == "Slut" ? "Sluttid" : "Starttid";
            changes.Add($"{timeLabel} er rykket fra {_dateTime.DanishTime(beforeUtc)} til {_dateTime.DanishTime(afterUtc)}.");
        }



    }
}
