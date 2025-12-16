using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Sir98Backend.Data;
using Sir98Backend.Interfaces;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;

namespace Sir98Backend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string email)
        {
            email = (email ?? string.Empty).Trim().ToLowerInvariant();

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task RegisterUserAsync(RegisterAccount newUser, string activationCode)
        {
            if (newUser is null) throw new ArgumentNullException(nameof(newUser));
            if (string.IsNullOrWhiteSpace(newUser.Email)) throw new Exception("Invalid registration request.");
            if (string.IsNullOrWhiteSpace(newUser.Password)) throw new Exception("Invalid registration request.");
            if (string.IsNullOrWhiteSpace(activationCode)) throw new Exception("Invalid registration request.");

            var email = newUser.Email.Trim().ToLowerInvariant();
            newUser.Email = email;

            // If ANY of these conditions fail, don't reveal which one.
            var alreadyUser = await _context.Users.AnyAsync(u => u.Email == email);
            var awaiting = await _context.UsersAwaitingActivation.AnyAsync(a => a.Email == email);
            var codeInUse = await _context.UsersAwaitingActivation.AnyAsync(a => a.ActivationCode == activationCode);

            Console.WriteLine(alreadyUser + " " + awaiting + " " + codeInUse);

            if (alreadyUser || awaiting || codeInUse)
                throw new Exception("Unable to process registration request.");

            var pending = new UserAwaitActivation
            {
                ActivationCode = activationCode,
                Email = email,
                HashedPassword = Argon2.Hash(newUser.Password),
                ExpirationDate = DateTime.UtcNow.AddMinutes(5),
            };

            _context.UsersAwaitingActivation.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ActivateUserAsync(string activationCode)
        {
            // Generic errors so callers can't probe valid/expired codes
            if (string.IsNullOrWhiteSpace(activationCode))
                throw new Exception("Invalid or expired activation code.");

            var pending = await _context.UsersAwaitingActivation
                .FirstOrDefaultAsync(x => x.ActivationCode == activationCode);

            if (pending is null)
                throw new Exception("Invalid or expired activation code.");

            if (pending.ExpirationDate < DateTime.UtcNow)
            {
                _context.UsersAwaitingActivation.Remove(pending);
                await _context.SaveChangesAsync();
                throw new Exception("Invalid or expired activation code.");
            }

            await using var tx = await _context.Database.BeginTransactionAsync();

            var userExists = await _context.Users.AnyAsync(u => u.Email == pending.Email);
            if (!userExists)
            {
                _context.Users.Add(new User
                {
                    Email = pending.Email,
                    HashedPassword = pending.HashedPassword,
                    Role = "Member",
                });
            }

            _context.UsersAwaitingActivation.Remove(pending);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return true;
        }
    }
}
