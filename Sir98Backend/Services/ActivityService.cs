using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;

namespace Sir98Backend.Services
{
    public class ActivityService
    {
        private readonly AppDbContext _context;
        private readonly NotificationService _notificationService;

        public ActivityService(AppDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
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

        public async Task<Activity?> PutAndNotifyAsync(int id, Activity activity)
        {
            var updated = await PutAsync(id, activity);
            if (updated == null)
                return null;

            var payload = new NotificationPayload
            {
                Title = $"{updated.Title} er blevet ændret.",
                Body = $"{updated.Title} på x dato er blevet ændret. Klik for at se mere.",
                Url = "http://localhost:5173/account-settings"
            };

            await _notificationService.NotifyUsersAboutSeriesChangeAsync(updated.Id, payload);

            return updated;
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
