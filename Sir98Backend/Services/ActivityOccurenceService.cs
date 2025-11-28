using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Repository;

namespace Sir98Backend.Services
{
    public class ActivityOccurrenceService
    {
        private readonly ActivityRepo _activityRepo;
        private readonly ChangedActivityRepo _changedActivityRepo;

        private static readonly TimeZoneInfo DanishZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

        private const string DanishTzId = "Europe/Copenhagen";

        public ActivityOccurrenceService(ActivityRepo activityRepo, ChangedActivityRepo changedActivityRepo)
        {
            _activityRepo = activityRepo;
            _changedActivityRepo = changedActivityRepo;
        }
        /// <summary>
        /// Gets all Activities and their recurrences from a specific date and x amount of days forward.
        /// Uses two helper methods. One for generating the recurrences and another one for looking up changedActivities and putting them into a Dto.
        /// </summary>
        /// <param name="fromUtc"></param> StartTime in UTC. Used so we can "get the next page" starting from right after the last StartTime shown
        /// <param name="days"></param> Amount of days forward we will get. For example 7 days forward means we will get from StartTime until 7 days later
        /// <returns></returns>
        public IEnumerable<ActivityOccurrenceDto> GetOccurrences(DateTimeOffset fromUtc, int days)
        {
            var toUtc = fromUtc.AddDays(days); //Calculates the EndDate based on how many days from the parameter

            var activities = _activityRepo.GetAll();
            var changes = _changedActivityRepo.GetAll();

            // (ActivityId, OriginalStartUtc) -> ChangedActivity
            var changeLookup = changes
                .GroupBy(c => (c.ActivityId, c.OriginalStartUtc))
                .ToDictionary(g => g.Key, g => g.First());

            var result = new List<ActivityOccurrenceDto>(); //Will contain all activities(including the recurrences) after ChangedActivities has been applied

            foreach (var activity in activities)
            {
                if (!activity.IsRecurring) //If activity is not recurring
                {
                    // Single event
                    if (activity.StartUtc >= fromUtc && activity.StartUtc < toUtc) //If the activity's Start is within the range from parameters, run it through helper method
                    {
                        AddOccurrence(activity, activity.StartUtc, activity.EndUtc, changeLookup, result);
                    }

                    continue;
                }

                if (string.IsNullOrWhiteSpace(activity.Rrule)) //Rrule should never be null on a recurring activity. Will be skipped to avoid crashes.
                {
                    continue;
                }

                // For every Activity, generate its recurrences. For every activity and recurrence, check if theres a ChangedActivity and convert it to the Dto.
                foreach (var (originalStartUtc, originalEndUtc) in
                         GenerateBaseOccurrences(activity, fromUtc, toUtc))
                {
                    AddOccurrence(activity, originalStartUtc, originalEndUtc, changeLookup, result);
                }
            }

            return result
                .OrderBy(o => o.StartUtc)
                .ToList();
        }

        /// <summary>
        /// Uses Ical.Net to expand the RRULE of a single Activity into UTC occurrences.
        /// </summary>
     private IEnumerable<(DateTimeOffset startUtc, DateTimeOffset endUtc)>
    GenerateBaseOccurrences(Activity activity, DateTimeOffset fromUtc, DateTimeOffset toUtc)
        {
            var fromLocal = TimeZoneInfo.ConvertTime(fromUtc, DanishZone).DateTime; //From today
            var toLocal = TimeZoneInfo.ConvertTime(toUtc, DanishZone).DateTime; //To today

            var baseStartLocal = TimeZoneInfo.ConvertTime(activity.StartUtc, DanishZone).DateTime; //Converts act start from utc to dk
            var baseEndLocal = TimeZoneInfo.ConvertTime(activity.EndUtc, DanishZone).DateTime; //Converts act end from utc to dk

            var calendarEvent = new CalendarEvent
            {
                DtStart = new CalDateTime(baseStartLocal, DanishTzId),
                DtEnd = new CalDateTime(baseEndLocal, DanishTzId)
            };

            calendarEvent.RecurrenceRules.Add(new RecurrencePattern(activity.Rrule)); //Adds the RRULE to the calendar event
           
            var fromLocalCal = new CalDateTime(fromLocal, DanishTzId); //Converts to CalDateTime
            var toLocalCal = new CalDateTime(toLocal, DanishTzId); //Converts to CalDateTime

            var occurrences = calendarEvent
                .GetOccurrences(fromLocalCal)       // Starts taking occurrences from here
                .TakeWhileBefore(toLocalCal);       // Starts taking occurrences until here


            foreach (var occ in occurrences) //For each occurence generated, we convert back to UTC and yield return
            {
                var occStartUtc = occ.Period.StartTime.AsUtc;
                var occEndUtc = occ.Period.EffectiveEndTime.AsUtc;

                yield return (new DateTimeOffset(occStartUtc),
                              new DateTimeOffset(occEndUtc));
            }
        }



        /// <summary>
        /// Combine Activity + ChangedActivity to produce a DTO for one occurrence.
        /// </summary>
        private void AddOccurrence(
            Activity activity,
            DateTimeOffset originalStartUtc,
            DateTimeOffset originalEndUtc,
            Dictionary<(int ActivityId, DateTimeOffset OriginalStartUtc), ChangedActivity> changeLookup,
            List<ActivityOccurrenceDto> result)
        {
            if (changeLookup.TryGetValue((activity.Id, originalStartUtc), out var change))
            {

                var start = change.NewStartUtc ?? originalStartUtc;
                var end = change.NewEndUtc ?? originalEndUtc;

                var title = change.NewTitle ?? activity.Title;
                var description = change.NewDescription ?? activity.Description;
                var address = change.NewAddress ?? activity.Address;
                var instructors = change.NewInstructors ?? activity.Instructors;
                var tags = change.NewTags ?? activity.Tags;

                // Cancelled if either the parent activity is cancelled, or this specific instance is
                var cancelled = activity.Cancelled || change.IsCancelled;

                result.Add(new ActivityOccurrenceDto
                {
                    ActivityId = activity.Id,
                    OriginalStartUtc = originalStartUtc,
                    StartUtc = start,
                    EndUtc = end,
                    Title = title,
                    Description = description,
                    Address = address ?? "",
                    Image = activity.Image,
                    Link = activity.Link,
                    Instructors = instructors,
                    Tags = tags ?? new List<string>(),
                    Cancelled = cancelled
                });
            }
            else
            {
                // No change: just use the activity data
                result.Add(new ActivityOccurrenceDto
                {
                    ActivityId = activity.Id,
                    OriginalStartUtc = originalStartUtc,
                    StartUtc = originalStartUtc,
                    EndUtc = originalEndUtc,
                    Title = activity.Title,
                    Description = activity.Description,
                    Address = activity.Address,
                    Image = activity.Image,
                    Link = activity.Link,
                    Instructors = activity.Instructors,
                    Tags = activity.Tags ?? new List<string>(),
                    Cancelled = activity.Cancelled
                });
            }
        }
    }
}
