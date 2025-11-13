using Sir98Backend.Models;
using System.Xml.Linq;

namespace Sir98Backend.Repository
{
    public class ActivityRepo
    {
        private List<Activity> _activity = new();
        private int _nextId = 1;



        public ActivityRepo()
        {

            var instructor1 = new Instructor
            {
                Id = 1,
                Email = "alice@example.com",
                Number = "12345678",
                FirstName = "Alice",
                Image = "alice.jpg"
            };

            var instructor2 = new Instructor
            {
                Id = 2,
                Email = "bob@example.com",
                Number = "87654321",
                FirstName = "Bob",
                Image = "bob.jpg"
            };



            _activity = new List<Activity>();
            _activity.Add(new Activity
            {
                Id = _nextId++,
                Title = "Badminton",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                Address = "Rosevej 8",
                Image = "billede.png",
                Link = "www.y8",
                Cancelled = false,
                Instructors = new List<Instructor> { instructor1 }
            });

            _activity.Add(new Activity
            {
                Id = _nextId++,
                Title = "Fodbold",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(3),
                Address = "Blomstergade 5",
                Image = "billede2.png",
                Link = "www.y9",
                Cancelled = false
            });

            _activity.Add(new Activity
            {
                Id = _nextId++,
                Title = "Svømning",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(1),
                Address = "Løvevej 3",
                Image = "billede3.png",
                Link = "www.y10",
                Cancelled = true,
                Instructors = new List<Instructor> { instructor2, instructor1 }
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
            existingActivity.Start = activity.Start;
            existingActivity.End = activity.End;
            existingActivity.Address = activity.Address;
            existingActivity.Image = activity.Image;
            existingActivity.Link = activity.Link;
            existingActivity.Cancelled = activity.Cancelled;
            return existingActivity;
        }
    }
}
