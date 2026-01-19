using Sir98Backend.Interfaces;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Repository;

namespace Sir98Backend.Services
{
    public sealed class OccurrenceSnapshotResolver : IOccurrenceSnapshotResolver
    {
        private readonly ActivityRepo _activityRepo;
        private readonly ChangedActivityRepo _changedRepo;

        public OccurrenceSnapshotResolver(
            ActivityRepo activityRepo,
            ChangedActivityRepo changedRepo)
        {
            _activityRepo = activityRepo;
            _changedRepo = changedRepo;
        }

        public async Task<OccurrenceSnapshot> ResolveAsync(int activityId, DateTimeOffset originalStartUtc)
        {
            var activity = await _activityRepo.GetByIdInclInstructorAsync(activityId)
                ?? throw new InvalidOperationException($"Activity {activityId} not found.");

            var change = await _changedRepo.GetByActivityAndOriginalStartAsync(activityId, originalStartUtc);

            // Base values come from activity
            var startUtc = activity.StartUtc;
            var endUtc = activity.EndUtc;
            var title = activity.Title;
            var description = activity.Description;
            var address = activity.Address;
            var tag = activity.Tag;
            var instructorIds = activity.Instructors
                .Select(i => i.Id)
                .ToList();

            var isCancelled = activity.Cancelled;

            // Apply overrides if we have a change
            if (change != null)
            {
                isCancelled = change.IsCancelled;

                startUtc = change.NewStartUtc ?? startUtc;
                endUtc = change.NewEndUtc ?? endUtc;
                title = change.NewTitle ?? title;
                description = change.NewDescription ?? description;
                address = change.NewAddress ?? address;
                tag = change.NewTag ?? tag;
                if (change.NewInstructors != null && change.NewInstructors.Count > 0)
                {
                    instructorIds = change.NewInstructors
                        .Select(i => i.Id)
                        .ToList();
                }
            }

            return new OccurrenceSnapshot(
                activityId: activity.Id,
                originalStartUtc: originalStartUtc,
                isCancelled: isCancelled,
                startUtc: startUtc,
                endUtc: endUtc,
                title: title,
                description: description,
                address: address,
                instructorIds: instructorIds,
                tag: tag
            );
        }

    }

}
