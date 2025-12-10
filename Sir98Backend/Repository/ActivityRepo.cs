using Sir98Backend.Models;
using System.Xml.Linq;

namespace Sir98Backend.Repository
{
    public class ActivityRepo
    {
        private List<Activity> _activity = new();
        private int _nextId = 1;

        private static readonly TimeZoneInfo DanishTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");



        public ActivityRepo()
        {

            var instructor1 = new Instructor
            {
                Id = 1,
                Email = "",
                Number = "",
                FirstName = "Sarah",
                Image = ""
            };

            var instructor2 = new Instructor
            {
                Id = 2,
                Email = "larsboh@roskilde.dk",
                Number = "24629361",
                FirstName = "Lars",
                Image = "hansBillede.png"
            };

            DateTimeOffset DkLocal(int year, int month, int day, int hour, int minute) //Will be deleted once we no longer use mockdata
            {
                var local = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Unspecified);
                var utc = TimeZoneInfo.ConvertTimeToUtc(local, DanishTimeZone);
                return new DateTimeOffset(utc, TimeSpan.Zero);
            }




            _activity = new List<Activity>();
            // --- Badminton – Wednesday 16:30–18:00 DK time ---
            Add(new Activity
            {
                Title = "Badminton",
                StartUtc = DkLocal(2025, 12, 3, 16, 30),
                EndUtc = DkLocal(2025, 12, 3, 18, 00),
                Address = "Hedegårdene Skole, Københavnsvej 34, 4000 Roskilde",
                Image = "badminton.png",
                Link = "https://if-sir98.dk/#badminton",
                Cancelled = false,
                Description = "Badmintontræning for alle niveauer.",
                IsRecurring = true,
                Tag = "Træning",
                Rrule = "FREQ=WEEKLY;BYDAY=WE"
            });

            // --- Cirkeltræning – Wednesday 14:15–15:00 DK time ---
            Add(new Activity
            {
                Title = "Cirkeltræning",
                StartUtc = DkLocal(2025, 12, 3, 14, 15),
                EndUtc = DkLocal(2025, 12, 3, 15, 00),
                Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                Image = "cirkeltraening.png",
                Link = "https://if-sir98.dk/#cirkeltr%C3%A6ning",
                Cancelled = false,
                Instructors = new List<Instructor> { instructor1, instructor2 },
                Description = "Cirkeltræning med fokus på hele kroppen.",
                Tag =  "Træning",
                IsRecurring = true,
                Rrule = "FREQ=WEEKLY;BYDAY=WE"
            });
            // --- Cirkeltræning – Friday 10:15–11:00 DK time ---
            Add(new Activity
            {
                Title = "Cirkeltræning",
                StartUtc = DkLocal(2025, 12, 5, 10, 15),
                EndUtc = DkLocal(2025, 12, 5, 11, 00),
                Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                Image = "cirkeltraening.png",
                Link = "https://if-sir98.dk/#cirkeltr%C3%A6ning",
                Cancelled = false,
                Instructors = new List<Instructor> { instructor2 },
                Description = "Formiddags-cirkeltræning.",
                Tag = "Træning",
                IsRecurring = true,
                Rrule = "FREQ=WEEKLY;BYDAY=FR"
            });

            // --- Styrke og sundhedstræning – Monday 14:30–16:00 DK time ---
            Add(new Activity
            {
                Title = "Styrke og sundhedstræning",
                StartUtc = DkLocal(2025, 12, 1, 14, 30),
                EndUtc = DkLocal(2025, 12, 1, 16, 00),
                Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                Image = "styrke.png",
                Link = "https://if-sir98.dk/#styrke-og-sundhedstr%C3%A6ning",
                Cancelled = false,
                Description = "Åbent i træningscenteret. Ingen instruktør.",
                Tag = "Træning" ,
                IsRecurring = true,
                Rrule = "FREQ=WEEKLY;BYDAY=MO"
            });

            // --- Styrke og sundhedstræning – Wednesday 14:00–16:00 DK time ---
            Add(new Activity
            {
                Title = "Styrke og sundhedstræning",
                StartUtc = DkLocal(2025, 12, 3, 14, 00),
                EndUtc = DkLocal(2025, 12, 3, 16, 00),
                Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                Image = "styrke.png",
                Link = "https://if-sir98.dk/#styrke-og-sundhedstr%C3%A6ning",
                Cancelled = false,
                Description = "Åbent i træningscenteret. Ingen instruktør.",
                Tag = "Træning",
                IsRecurring = true,
                Rrule = "FREQ=WEEKLY;BYDAY=WE"
            });

            // --- Styrke og sundhedstræning – Friday 10:00–12:00 DK time ---
            Add(new Activity
            {
                Title = "Styrke og sundhedstræning",
                StartUtc = DkLocal(2025, 12, 5, 10, 00),
                EndUtc = DkLocal(2025, 12, 5, 12, 00),
                Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                Image = "styrke.png",
                Link = "https://if-sir98.dk/#styrke-og-sundhedstr%C3%A6ning",
                Cancelled = false,
                Description = "Åbent i træningscenteret. Ingen instruktør.",
                  Tag = "Træning",
                IsRecurring = true,
                Rrule = "FREQ=WEEKLY;BYDAY=FR"
            });

            Add(new Activity
            {
                Title = "BrugerTests",
                StartUtc = DkLocal(2025, 12, 3, 14, 15),
                EndUtc = DkLocal(2025, 12, 3, 15, 00),
                Address = "RMI, Store Møllevej 5, 4000 Roskilde",
                Image = "tests.png",
                Link = "https://if-sir98.dk/#styrke-og-sundhedstr%C3%A6ning",
                Cancelled = false,
                Description = "Åbent i træningscenteret. Ingen instruktør.",
                 Tag = "Træning",
                IsRecurring = false,
                
            });

        }

        public List<Activity> GetAll()
        {
            return _activity;
        }

        public Activity? GetById(int id)
        {
            return _activity.Find(e => e.Id == id);
        }

        public Activity Add(Activity activity)
        {
            activity.Id = _nextId++;
            _activity.Add(activity);
            return activity;
        }

        public Activity? Delete(int id)
        {
            var activity = GetById(id);
            if (activity == null)
            {
                return null;
            }
            _activity.Remove(activity);
            return activity;
        }

        public Activity? Update(int id, Activity activity)
        {
            Activity? existingActivity = GetById(id);
            if (existingActivity == null)
            {
                return null;
            }
            existingActivity.Title = activity.Title;
            existingActivity.StartUtc = activity.StartUtc;
            existingActivity.EndUtc = activity.EndUtc;
            existingActivity.Address = activity.Address;
            existingActivity.Image = activity.Image;
            existingActivity.Link = activity.Link;
            existingActivity.Cancelled = activity.Cancelled;
            existingActivity.Description = activity.Description;
            existingActivity.Instructors = activity.Instructors;
            existingActivity.Tag = activity.Tag;
            existingActivity.IsRecurring = activity.IsRecurring;
            existingActivity.Rrule = activity.Rrule;
            return existingActivity;
        }
    }
}
