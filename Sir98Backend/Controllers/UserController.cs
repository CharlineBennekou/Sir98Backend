using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sir98Backend.Models;
using Sir98Backend.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Sir98Backend.Models.DataTransferObjects;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.RateLimiting;

namespace Sir98Backend.Controllers
{
    [ApiController]
    [EnableRateLimiting("userLoginRegisterForgot")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepo _userRepo;
        public UserController(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("Register")]
        public IActionResult RegisterAccount()
        {
            throw new Exception();
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            Console.WriteLine(credentials.Email);
            if(IsPasswordValid(credentials.Password) == false)
            {
                return Unauthorized("Invalid password");
            }
            User user = _userRepo.GetUser(credentials.Email.ToLower());
            if(user is null || user is default(User))
            {
                return Unauthorized("User not found");
            }

            if (Argon2.Verify(user.HashedPassword, credentials.Password) == false)
            {
                return Unauthorized("User not found");
            }
            string filepath = Environment.CurrentDirectory + "\\Keys\\JWToken key for signing.txt";
            string keyForSigning = System.IO.File.ReadAllText(filepath);
            return Ok($"Bearer {GenerateJWToken(user, keyForSigning)}");
        }

        private bool IsPasswordValid(string password)
        {
            return true;
        }

        private string GenerateJWToken(User user, string JWTokenSigningKey)
        {
            // authentication successful so generate jwt token
            if(user is null || user is default(User))
            {
                throw new ArgumentNullException("user can not be null");
            }
            if(JWTokenSigningKey is null || JWTokenSigningKey is default(string))
            {
                throw new ArgumentNullException("Signing key can not be null");
            }

            var key = Encoding.ASCII.GetBytes(JWTokenSigningKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Name, user.Email.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                ]),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("ActivationLink")]
        public IActionResult ActivationLink()
        {
            throw new Exception();
        }


        [HttpPost("ForgotPassword")]
        public IActionResult SendForgotPassword()
        {
            throw new Exception();
        }
    }
}
