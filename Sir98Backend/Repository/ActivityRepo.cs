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

        /// <summary>
        /// Returns all activities without including related entities. Write "Incl" methods to include related entities.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Activity>> GetAllAsync()
    => await _context.Activities
        .AsNoTracking()
        .ToListAsync();

        /// <summary>
        /// Includes Activity by its Id without including related entities. Write "Incl" methods to include related entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Activity?> GetByIdAsync(int id)
            => await _context.Activities
                .AsNoTracking()
                .Include(a => a.Instructors)
                .FirstOrDefaultAsync(a => a.Id == id);

        /// <summary>
        /// Includes all activities including their related Instructors.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Activity>> GetAllInclInstructorsAsync()
    => await _context.Activities
        .AsNoTracking()
        .Include(a => a.Instructors)
        .ToListAsync();
        /// <summary>
        /// Includes Activity by its Id including related Instructors.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Activity?> GetByIdInclInstructorAsync(int id)
            => await _context.Activities
                .AsNoTracking()
                .Include(a => a.Instructors)
                .FirstOrDefaultAsync(a => a.Id == id);

        /// <summary>
        /// Returns all activities including their related Instructors, ActivitySubscriptions and ChangedActivity.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Activity>> GetAllInclAllAsync()
   => await _context.Activities
       .AsNoTracking()
       .Include(a => a.Instructors)
       .Include(a => a.ActivitySubscriptions)
       .Include(a => a.ChangedActivity)
       .ToListAsync();
        /// <summary>
        /// Returns Activity by its Id including related Instructors, ActivitySubscriptions and ChangedActivity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Activity?> GetByIdInclAllAsync(int id)
            => await _context.Activities
                .AsNoTracking()
                .Include(a => a.Instructors)
                .Include(a => a.ActivitySubscriptions)
                .Include(a => a.ChangedActivity)
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
                //to update instrcutors they must be included
                .Include(a => a.Instructors)
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
            existing.Tag = activity.Tag;
            existing.IsRecurring = activity.IsRecurring;
            existing.Rrule = activity.Rrule;

            //remove previous instructors and add the new ones
            existing.Instructors.Clear();
            foreach(var instructor in activity.Instructors)
            {
                existing.Instructors.Add(instructor);
            }

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
