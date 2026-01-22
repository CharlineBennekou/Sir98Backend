using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Repository;

namespace Sir98Backend.Services
{
    public class ActivityOccurrenceService
    {
        private readonly ActivityRepo _activityRepo;
        private readonly ChangedActivityRepo _changedActivityRepo;
        private readonly ActivitySubscriptionRepo _activitySubsRepo;


        private static readonly TimeZoneInfo DanishZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

        private const string DanishTzId = "Europe/Copenhagen";

        public ActivityOccurrenceService(ActivityRepo activityRepo, ChangedActivityRepo changedActivityRepo, ActivitySubscriptionRepo subscriptionrepo)
        {
            _activityRepo = activityRepo;
            _changedActivityRepo = changedActivityRepo;
            _activitySubsRepo = subscriptionrepo;
        }
        /// <summary>
        /// Gets all Activities and their recurrences from a specific date and x amount of days forward.
        /// Uses two helper methods. One for generating the recurrences and another one for looking up changedActivities and putting them into a Dto.
        /// </summary>
        /// <param name="fromUtc"></param> StartTime in UTC. Used so we can "get the next page" starting from right after the last StartTime shown
        /// <param name="days"></param> Amount of days forward we will get. For example 7 days forward means we will get from StartTime until 7 days later
        /// <returns></returns>
        public async Task<IEnumerable<ActivityOccurrenceDto>> GetOccurrencesAsync(DateTimeOffset fromUtc, int days, string filter, string userId)
        {
            var toUtc = fromUtc.AddDays(days); //Calculates the EndDate based on how many days from the parameter

            IEnumerable<Activity> activities = await _activityRepo.GetAllInclInstructorsAsync(); //Gets all activities from repo



            if (!string.IsNullOrWhiteSpace(filter))
            {
                string normalizedFilter = filter.Trim().ToLower();

                // if filter is "mine" and userId is provided, return only subscribed activities(subscribed to series or to an occurrence in the serie)
                if (normalizedFilter == "mine" && userId != null)
                {
                    var subscribedActivityIds = (await _activitySubsRepo.GetByUserIdAsync(userId))
                        .Select(s => s.ActivityId)
                        .ToHashSet();

                    activities = activities
                        .Where(a => subscribedActivityIds.Contains(a.Id))
                        .ToList();
                }
                else
                {
                    // filter by tags containing the filter string
                    activities = activities
                      .Where(a =>
                       !string.IsNullOrWhiteSpace(a.Tag) &&
                       a.Tag.ToLower().Contains(normalizedFilter))
                      .ToList();
                }
            }




            var changes = await _changedActivityRepo.GetAllInclInstructorsAsync();

            // (ActivityId, OriginalStartUtc) -> ChangedActivity
            var changeLookup = changes
                .GroupBy(c => (c.ActivityId, c.OriginalStartUtc))
                .ToDictionary(g => g.Key, g => g.First());

            var result = new List<ActivityOccurrenceDto>(); //Will contain all activities(including the recurrences) after ChangedActivities has been applied

            foreach (var activity in activities)
            {
                if (!activity.IsRecurring) //If activity is not recurring
                {

                    AddOccurrence(activity, activity.StartUtc, activity.EndUtc, changeLookup, result);


                    continue;
                }

                if (string.IsNullOrWhiteSpace(activity.Rrule)) //Rrule should never be null on a recurring activity. Will be skipped to avoid crashes.
                    continue;


                // For every Activity, generate its recurrences. For every activity and recurrence, check if theres a ChangedActivity and convert it to the Dto.
                foreach (var (originalStartUtc, originalEndUtc) in
                         GenerateBaseOccurrences(activity, fromUtc, toUtc))
                {
                    AddOccurrence(activity, originalStartUtc, originalEndUtc, changeLookup, result);
                }
            }
            if (userId != null)
            {
                await SetToSubscribed(result, userId);
            }
            if (filter == "mine")
            {
                result = result
                    .Where(o => o.IsSubscribed)
                    .ToList();
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
            Console.WriteLine(activity.Id);
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

            Console.WriteLine(fromLocalCal.ToString());
            Console.WriteLine(toLocalCal.ToString());

            var occurrences = calendarEvent
                .GetOccurrences(fromLocalCal)       // Starts taking occurrences from here
                .TakeWhileBefore(toLocalCal).ToList();       // Starts taking occurrences until here


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
            ChangedActivity? change;
            bool hasChange = changeLookup.TryGetValue((activity.Id, originalStartUtc), out change);

            DateTimeOffset start = hasChange && change!.NewStartUtc.HasValue ? change.NewStartUtc.Value : originalStartUtc;
            DateTimeOffset end = hasChange && change!.NewEndUtc.HasValue ? change.NewEndUtc.Value : originalEndUtc;

            string title = hasChange && !string.IsNullOrWhiteSpace(change!.NewTitle) ? change.NewTitle! : activity.Title;
            string? description = hasChange ? (change!.NewDescription ?? activity.Description) : activity.Description;
            string? address = hasChange ? (change!.NewAddress ?? activity.Address) : activity.Address;
            string? tag = hasChange ? (change!.NewTag ?? activity.Tag) : activity.Tag;

            IEnumerable<Instructor>? instructors = hasChange ? (change!.NewInstructors ?? activity.Instructors) : activity.Instructors;
            List<InstructorDto> instructorDtos = (instructors ?? Enumerable.Empty<Instructor>())
                .Select(i => new InstructorDto
                {
                    Id = i.Id,
                    FirstName = i.FirstName,
                    Email = i.Email,
                    Number = i.Number,
                    Image = i.Image
                })
                .ToList();

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
                Instructors = instructorDtos,
                Tag = tag ?? "",
                Cancelled = hasChange ? change!.IsCancelled : activity.Cancelled,
                IsRecurring = activity.IsRecurring

            });
        }


        private async Task SetToSubscribed(List<ActivityOccurrenceDto> result, string userId)
        {
            IEnumerable<ActivitySubscription> subs =
                await _activitySubsRepo.GetByUserIdAsync(userId);

            HashSet<int> allOccurrenceActivityIds = subs
                .Where(s => s.AllOccurrences)
                .Select(s => s.ActivityId)
                .ToHashSet();

            HashSet<(int ActivityId, DateTimeOffset OriginalStartUtc)> singleOccurrenceKeys = subs
                .Where(s => !s.AllOccurrences)
                .Select(s => (s.ActivityId, s.OriginalStartUtc))
                .ToHashSet();

            foreach (ActivityOccurrenceDto occurrence in result)
            {
                occurrence.IsSubscribed =
                   allOccurrenceActivityIds.Contains(occurrence.ActivityId)
                   || singleOccurrenceKeys.Contains((occurrence.ActivityId, occurrence.OriginalStartUtc!.Value));

            }
        }









    }

}
