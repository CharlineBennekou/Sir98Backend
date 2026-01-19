using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Repository;

namespace Sir98Backend.Services
{
    public class ActivityService
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly ActivityNotificationPayloadBuilder _payloadBuilder;
        private readonly ActivityRepo _activityRepo;

        public ActivityService(AppDbContext context, NotificationService notificationService, ActivityNotificationPayloadBuilder payloadBuilder, ActivityRepo activityRepo)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _payloadBuilder = payloadBuilder ?? throw new ArgumentNullException(nameof(payloadBuilder));
            _activityRepo = activityRepo ?? throw new ArgumentNullException(nameof(activityRepo));
        }

        public async Task<Activity> CreateAsync(Activity activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return activity;
        }

        public async Task<Activity?> PutAsync(int id, Activity activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));

            var existing = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existing == null)
                return null;

            // Copy incoming values onto the tracked entity
            _context.Entry(existing).CurrentValues.SetValues(activity);
            existing.Id = id; // ensure key stays correct

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Activity?> DeleteAsync(int id)
        {
            var existing = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existing == null)
                return null;

            _context.Activities.Remove(existing);
            await _context.SaveChangesAsync();

            return existing;
        }

        public Task<Activity?> GetByIdAsync(int id)
            => _context.Activities.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<Activity>> GetAllAsync()
            => await _context.Activities.ToListAsync();



        // ---- New methods: update/delete + notify ----

        //public async Task<Activity?> PutAndNotifyAsync(int id, Activity activity)
        //{
        //    var updated = await _activityRepo.UpdateAsync(id, activity);
        //    if (updated == null)
        //        return null;

        //    var payload = _payloadBuilder.BuildSeriesChange(updated);

        //    await _notificationService.NotifyUsersAboutSeriesChangeAsync(updated.Id, payload);

        //    return updated;
        //}
        public async Task<Activity?> PutAndNotifyAsync(int id, Activity activity)
        {
            //Load from database so we have a before
            var existing = await _activityRepo.GetByIdAsync(id);
            if (existing == null)
                return null;
            var beforeUpdate = await MapToOccurrenceSnapshot(existing);

            //Update
            var updated = await _activityRepo.UpdateAsync(id, activity);
            if (updated == null)
                return null;

            //Save the after
            OccurrenceSnapshot afterUpdate = await MapToOccurrenceSnapshot(activity);
            //It is a series if tag = Series. Otherwise it's a one time event.
            bool IsSeries = updated.Tag == "Træning";
            var payload = _payloadBuilder.BuildUpdatePayload(beforeUpdate, afterUpdate, IsSeries);

            await _notificationService.NotifyUsersAboutSeriesChangeAsync(updated.Id, payload);

            return updated;
        }

        public Task<OccurrenceSnapshot> MapToOccurrenceSnapshot(Activity activity)
        {
            var snapshot = new OccurrenceSnapshot(
                activityId: activity.Id,
                originalStartUtc: null,
                isCancelled: false,
                startUtc: activity.StartUtc,
                endUtc: activity.EndUtc,
                title: activity.Title,
                description: activity.Description,
                address: activity.Address,
                instructorIds: activity.Instructors
                    .Select(i => i.Id)
                    .ToList(),
                tag: activity.Tag
            );

            return Task.FromResult(snapshot);
        }


        public async Task<Activity?> DeleteAndNotifyAsync(int id)
        {
            var deleted = await DeleteAsync(id);
            if (deleted == null)
                return null;

            var payload = new NotificationPayload
            {
                Title = $"{deleted.Title} er blevet ændret.",
                Body = $"{deleted.Title} på x dato er blevet ændret. Klik for at se mere.",
                Url = "http://localhost:5173/account-settings"
            };

            await _notificationService.NotifyUsersAboutSeriesChangeAsync(deleted.Id, payload);

            return deleted;
        }
    }
}
