using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sir98Backend.Models;
using Sir98Backend.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Sir98Backend.Models.DataTransferObjects;
using Microsoft.AspNetCore.RateLimiting;

namespace Sir98Backend.Controllers
{
    [ApiController]
    [EnableRateLimiting("userLoginRegisterForgot")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepo _userRepo;

        public UserController(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAccount()
        {
            // Placeholder for future DB/email work
            await Task.Yield();
            throw new Exception();
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            if (credentials == null)
                return BadRequest("Body is required.");

            if (string.IsNullOrWhiteSpace(credentials.Email) || string.IsNullOrWhiteSpace(credentials.Password))
                return BadRequest("Email and password are required.");

            Console.WriteLine(credentials.Email);

            if (!IsPasswordValid(credentials.Password))
                return Unauthorized("Invalid password");

            var user = await _userRepo.GetUserAsync(credentials.Email.ToLower());
            if (user == null)
                return Unauthorized("User not found");

            if (!Argon2.Verify(user.HashedPassword, credentials.Password))
                return Unauthorized("Invalid password");

            string filepath = Path.Combine(
                Environment.CurrentDirectory,
                "Keys",
                "JWToken key for signing.txt"
            );

            string keyForSigning = await System.IO.File.ReadAllTextAsync(filepath);

            return Ok($"Bearer {GenerateJWToken(user, keyForSigning)}");
        }

        private bool IsPasswordValid(string password)
        {
            return true;
        }

        private string GenerateJWToken(User user, string JWTokenSigningKey)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(JWTokenSigningKey))
                throw new ArgumentNullException(nameof(JWTokenSigningKey));

            var key = Encoding.ASCII.GetBytes(JWTokenSigningKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("ActivationLink")]
        public async Task<IActionResult> ActivationLink()
        {
            // Placeholder for future DB/email work
            await Task.Yield();
            throw new Exception();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> SendForgotPassword()
        {
            // Placeholder for future DB/email work
            await Task.Yield();
            throw new Exception();
        }
    }
}
