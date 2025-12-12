using Sir98Backend.Models;
using System.Xml.Linq;
using Sir98Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Sir98Backend.Repository
{
    public class ActivityRepo
    {
        private readonly AppDbContext _context;

        public ActivityRepo(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<Activity>> GetAllAsync()
            => await _context.Activities
                .AsNoTracking()
                .ToListAsync();

        
        public async Task<Activity?> GetByIdAsync(int id)
            => await _context.Activities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Activity> AddAsync(Activity activity)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<Activity?> DeleteAsync(int id)
        {
            var existing = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existing == null) return null;

            _context.Activities.Remove(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<Activity?> UpdateAsync(int id, Activity activity)
        {
            var existing = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existing == null) return null;

            existing.Title = activity.Title;
            existing.StartUtc = activity.StartUtc;
            existing.EndUtc = activity.EndUtc;
            existing.Address = activity.Address;
            existing.Image = activity.Image;
            existing.Link = activity.Link;
            existing.Cancelled = activity.Cancelled;
            existing.Description = activity.Description;
            existing.Instructors = activity.Instructors;
            existing.Tag = activity.Tag;
            existing.IsRecurring = activity.IsRecurring;
            existing.Rrule = activity.Rrule;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
