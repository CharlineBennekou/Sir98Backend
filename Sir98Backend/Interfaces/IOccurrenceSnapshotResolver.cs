using Sir98Backend.Models.DataTransferObjects;

namespace Sir98Backend.Interfaces
{
    public interface IOccurrenceSnapshotResolver
    {
        Task<OccurrenceSnapshot> ResolveAsync(int activityId, DateTimeOffset originalStartUtc);
    }
}
