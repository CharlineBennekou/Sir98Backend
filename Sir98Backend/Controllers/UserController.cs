using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Sir98Backend.Interfaces;
using Sir98Backend.Models;
using Sir98Backend.Models.DataTransferObjects;
using Sir98Backend.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace Sir98Backend.Controllers
{
    [ApiController]
    [EnableRateLimiting("userLoginRegisterForgot")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly IUserService _userService;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserController(
            IUserService userService,
            TokenService tokenService,
            EmailService emailService,
            IConfiguration configuration)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterAccount registration)
        {
            // Generic response to prevent user enumeration
            const string genericResponse = "If the email is eligible, an activation email has been sent.";

            if (registration.Password != registration.PasswordRepeated)
                return Ok(genericResponse);

            // We do NOT check "does user exist" here anymore (that leaks).
            string activationToken = _tokenService.GenerateActivationToken();

            try
            {
                await _userService.RegisterUserAsync(registration, activationToken);

                string link =
                    $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/User/Activate/code={activationToken}";

                MailMessage msg = _emailService.CreateEmail(
                    registration.Email,
                    "Activate your account",
                    link
                );
                _emailService.Send(msg);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                // Intentionally swallow details: still return generic response
                // (You should log the exception internally)
            }

            return Ok(genericResponse);
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            const string invalidAuth = "Invalid email or password.";

            if (!IsPasswordValid(credentials.Password))
                return Unauthorized(invalidAuth);

            var user = await _userService.GetUserAsync(credentials.Email);
            if (user == null)
                return Unauthorized(invalidAuth);

            if (!Argon2.Verify(user.HashedPassword, credentials.Password))
                return Unauthorized(invalidAuth);

            string signingKey = _configuration.GetValue<string>("JwtSettings:SigningKey");
            return Ok($"Bearer {_tokenService.GenerateJWToken(user, signingKey)}");
        }

        [HttpGet("Activate/code={code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActivationLink(string code)
        {
            try
            {
                await _userService.ActivateUserAsync(code);
                return Ok("User activated.");
            }
            catch
            {
                return BadRequest("Invalid or expired activation code.");
            }
        }

        private bool IsPasswordValid(string password) => true;
    }
}