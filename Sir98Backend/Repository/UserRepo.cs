using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Models;

namespace Sir98Backend.Repository
{
    public class UserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == email);
        }
    }
}
