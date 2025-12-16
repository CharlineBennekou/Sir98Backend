using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;

namespace Sir98Backend.Repository
{
    public class InstructorRepo
    {
        private readonly AppDbContext _context;

        public InstructorRepo(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Instructor>> GetAllAsync()
        {
            return _context.Instructors
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Instructor?> GetByIdAsync(int id)
        {
            return _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Instructor> AddAsync(Instructor instructor)
        {
            if (instructor == null)
                throw new ArgumentNullException(nameof(instructor));

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return instructor;
        }

        public async Task<Instructor?> DeleteAsync(int id)
        {
            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null)
                return null;

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();

            return instructor;
        }

        public async Task<Instructor?> UpdateAsync(int id, Instructor instructor)
        {
            var existing = await _context.Instructors
                .FirstOrDefaultAsync(i => i.Id == id);

            if (existing == null)
                return null;

            existing.Email = instructor.Email;
            existing.Number = instructor.Number;
            existing.FirstName = instructor.FirstName;
            existing.Image = instructor.Image;

            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
