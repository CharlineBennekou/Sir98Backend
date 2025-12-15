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
using Sir98Backend.Services;
using System.Net.Mail;

namespace Sir98Backend.Controllers
{
    [ApiController]
    [EnableRateLimiting("userLoginRegisterForgot")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepo _userRepo;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserController(UserRepo userRepo, TokenService tokenService, EmailService emailService, IConfiguration configuration)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RegisterAccount([FromBody] RegisterAccount registration)
        { 
            if (registration.Password != registration.PasswordRepeated)
            {
                return Unauthorized("Password does not match with repeated password");
            }
            User user = _userRepo.GetUser(registration.Email);
            if(user is not null || user is not default(User))
            {
                return Unauthorized("User with that email already exist");
            }
            string activationToken = _tokenService.GenerateActivationToken();
    
            string hashedPassword = Argon2.Hash(registration.Password);
    
            _userRepo.RegisterUser(registration, activationToken);
    
            string link = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/User/Activate/code={activationToken}";
    
            MailMessage test = _emailService.CreateEmail(registration.Email, "Test email", 
                $"{link}"
                );
            _emailService.Send(test);
    
            return Ok("Email has been sent if you have an account");
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            if(IsPasswordValid(credentials.Password) == false)
            {
                return Unauthorized("Invalid password");

            var user = await _userRepo.GetUserAsync(credentials.Email.ToLower());
            if (user == null)
                return Unauthorized("User not found");

            if (Argon2.Verify(user.HashedPassword, credentials.Password) == false)
            {
                return Unauthorized("User not found");
            }
            string keyForSigning = _configuration.GetValue<string>("JwtSettings:SigningKey");
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

        [HttpGet("Activate/code={code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActivationLink(string code)
        {
            try
            {
                _userRepo.ActivateUser(code);
            } catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok("User activated");
        }    
    }
}
