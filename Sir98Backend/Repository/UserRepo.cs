using Isopoh.Cryptography.Argon2;
using Sir98Backend.Models;
using System.Data;

namespace Sir98Backend.Repository
{
    public class UserRepo
    {
        private readonly ICollection<User> Users;

        public UserRepo()
        {
            Users = new List<User>(){
                new() {
                    Email = "bente@sørensen.com",
                    HashedPassword = Argon2.Hash("AdgangskodeTilSIR98"),
                    Role = "Member"
                },
                new() {
                    Email = "henborg@roskilde.dk",
                    HashedPassword = Argon2.Hash("HNielsen123!"),
                    Role = "Instructor"
                },
                new() {
                    Email = "admin@roskilde.dk",
                    HashedPassword = Argon2.Hash("AmkOFOod78#"),
                    Role = "UserAdmin"
                }
            };
        }

        public User? GetUser(string email)
        {
            return Users.FirstOrDefault((User user) => user.Email.Equals(email));
        }

        public void RegisterUser(RegisterAccount newUser, string activationCode)
        {
            newUser.Email = newUser.Email.ToLower();
            if (EmailsAwaitingActivation.Exists(user => user.ActivationCode.Equals(activationCode)))
            {
                throw new Exception("Activation code already in use");
            }
            if(EmailsAwaitingActivation.Exists(user => user.Email.Equals(newUser.Email)))
            {
                throw new Exception("User awaiting activation");
            }
        
            UserAwaitActivation user = new()
            {
                ActivationCode = activationCode,
                Email = newUser.Email.ToLower(),
                HashedPassword = Argon2.Hash(newUser.Password),
                ExpirationDate = DateTime.Now.AddMinutes(5),
            };
        
            EmailsAwaitingActivation.Add(user);
        }
        
        public bool ActivateUser(string activationCode)
        {
            UserAwaitActivation? user = EmailsAwaitingActivation.FirstOrDefault(user => user.ActivationCode.Equals(activationCode));
        
            if(user is null || user is default(UserAwaitActivation))
            {
                throw new Exception("activationCode is invalid");
            }
            DateTime now = DateTime.Now;
            if(user.ExpirationDate <  now)
            {
                EmailsAwaitingActivation.Remove(user);
                throw new Exception("activationCode has expired");
            }
            EmailsAwaitingActivation.Remove(user);
            Users.Add(new User()
            {
                Email = user.Email,
                HashedPassword = user.HashedPassword,
                Role = "Member",
            });
            return true;
        }
    }
}
