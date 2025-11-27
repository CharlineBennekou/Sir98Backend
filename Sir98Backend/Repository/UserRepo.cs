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
    }
}
