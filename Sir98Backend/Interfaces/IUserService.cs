using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;

namespace Sir98Backend.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(string email);

        Task RegisterUserAsync(RegisterAccount newUser, string activationCode);

        Task<bool> ActivateUserAsync(string activationCode);
    }
}
