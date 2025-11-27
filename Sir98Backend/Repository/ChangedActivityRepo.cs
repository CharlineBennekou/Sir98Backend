using Sir98Backend.Models;

namespace Sir98Backend.Repository
{
    public class ChangedActivityRepo
    {
        private readonly List<ChangedActivity> _changedActivities = new();
        private int _nextId = 1;

        public ChangedActivityRepo()
        {
            // 1) Cancel one specific Friday Cirkeltræning occurrence
            //
            // Base event: 2025-12-05 10:15 DK time = 2025-12-05 09:15 UTC
            // We cancel the occurrence on Friday 2025-12-19 at the same local time:
            // 2025-12-19 10:15 DK time = 2025-12-19 09:15 UTC

            _changedActivities.Add(new ChangedActivity
            {
                Id = _nextId++,
                ActivityId = 3, // Cirkeltræning (Friday) - adjust if needed
                OriginalStartUtc = new DateTimeOffset(2025, 12, 19, 9, 15, 0, TimeSpan.Zero),
                IsCancelled = true,

                NewStartUtc = null,
                NewEndUtc = null,
                NewTitle = null,
                NewDescription = null,
                NewAddress = null,
                NewInstructors = null,
            });

            // 2) Change the title (and optionally description) of one Monday Styrke occurrence
            //
            // Base event: 2025-12-01 14:30 DK time = 2025-12-01 13:30 UTC
            // We modify the next Monday: 2025-12-08 14:30 DK time = 2025-12-08 13:30 UTC

            _changedActivities.Add(new ChangedActivity
            {
                Id = _nextId++,
                ActivityId = 4, // Styrke og sundhedstræning (Monday) - adjust if needed
                OriginalStartUtc = new DateTimeOffset(2025, 12, 8, 13, 30, 0, TimeSpan.Zero),
                IsCancelled = false,

                NewStartUtc = null, // time unchanged
                NewEndUtc = null,
                NewTitle = "Styrke og sundhedstræning – Jule-special",
                NewDescription = "Jule-special i træningscenteret med ekstra hygge.",
                NewAddress = null,      // same address as parent
                NewInstructors = null,  // same instructors as parent
            });
        }

        // --- Basic CRUD-like operations ---

        public List<ChangedActivity> GetAll()
        {
            return _changedActivities;
        }

        public ChangedActivity? GetById(int id)
        {
            return _changedActivities.Find(c => c.Id == id);
        }

        public ChangedActivity Add(ChangedActivity change)
        {
            change.Id = _nextId++;
            _changedActivities.Add(change);
            return change;
        }

        public ChangedActivity? Delete(int id)
        {
            var existing = GetById(id);
            if (existing == null)
            {
                return null;
            }

            _changedActivities.Remove(existing);
            return existing;
        }

        public ChangedActivity? Update(int id, ChangedActivity change)
        {
            var existing = GetById(id);
            if (existing == null)
            {
                return null;
            }

            existing.ActivityId = change.ActivityId;
            existing.OriginalStartUtc = change.OriginalStartUtc;
            existing.IsCancelled = change.IsCancelled;
            existing.NewStartUtc = change.NewStartUtc;
            existing.NewEndUtc = change.NewEndUtc;
            existing.NewTitle = change.NewTitle;
            existing.NewDescription = change.NewDescription;
            existing.NewAddress = change.NewAddress;
            existing.NewInstructors = change.NewInstructors;
            // existing.NewTags       = change.NewTags; // if you have a NewTags property

            return existing;
        }
    }
}
