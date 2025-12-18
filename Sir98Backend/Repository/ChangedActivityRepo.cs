using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;

namespace Sir98Backend.Repository
{
    public class ChangedActivityRepo
    {
        private readonly AppDbContext _context;

        public ChangedActivityRepo(AppDbContext context)
        {
            _context = context;
        }

        // --- Basic CRUD-like operations ---

        public async Task<List<ChangedActivity>> GetAllAsync()
        {
            return await _context.ChangedActivities
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ChangedActivity>> GetAllInclInstructorsAsync()
        {
            return await _context.ChangedActivities
                .AsNoTracking()
                .Include(c => c.NewInstructors)
                .ToListAsync();
        }


        public async Task<ChangedActivity?> GetByIdAsync(int id)
        {
            return await _context.ChangedActivities
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ChangedActivity> AddAsync(ChangedActivity change)
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change));

            _context.ChangedActivities.Add(change);
            await _context.SaveChangesAsync();

            return change;
        }

        public async Task<ChangedActivity?> DeleteAsync(int id)
        {
            var existing = await _context.ChangedActivities
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existing == null)
                return null;

            _context.ChangedActivities.Remove(existing);
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<ChangedActivity?> Update(int id, ChangedActivity change)
        {
            var existing = await _context.ChangedActivities
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existing == null)
                return null;

            existing.ActivityId = change.ActivityId;
            existing.OriginalStartUtc = change.OriginalStartUtc;
            existing.IsCancelled = change.IsCancelled;
            existing.NewStartUtc = change.NewStartUtc;
            existing.NewEndUtc = change.NewEndUtc;
            existing.NewTitle = change.NewTitle;
            existing.NewDescription = change.NewDescription;
            existing.NewAddress = change.NewAddress;
            existing.NewInstructors = change.NewInstructors;
            // existing.NewTags = change.NewTags; // if applicable

            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
