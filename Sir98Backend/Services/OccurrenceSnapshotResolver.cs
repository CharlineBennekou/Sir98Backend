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

            var instructorIds =
                (change?.NewInstructors is { Count: > 0 })
                    ? change.NewInstructors.Select(i => i.Id).ToList()
                    : activity.Instructors.Select(i => i.Id).ToList();

            return new OccurrenceSnapshot(
                activityId: activity.Id,
                originalStartUtc: originalStartUtc,
                isCancelled: change?.IsCancelled ?? activity.Cancelled,
                startUtc: change?.NewStartUtc ?? activity.StartUtc,
                endUtc: change?.NewEndUtc ?? activity.EndUtc,
                title: change?.NewTitle ?? activity.Title,
                description: change?.NewDescription ?? activity.Description,
                address: change?.NewAddress ?? activity.Address,
                instructorIds: instructorIds,
                tag: change?.NewTag ?? activity.Tag
            );
        }


    }

}
