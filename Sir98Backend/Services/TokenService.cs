using Isopoh.Cryptography.Argon2;
using Microsoft.IdentityModel.Tokens;
using Sir98Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sir98Backend.Services
{
    public class TokenService
    {
        public string GenerateActivationToken()
        {
            var token = Guid.NewGuid();
            string hashedToken = Argon2.Hash(token.ToString());
            string onlyHashedToken = hashedToken.Substring(29);
            string onlyHashedTokenWithoutPlus = onlyHashedToken.Replace("+", "").Replace("/", "");
            return onlyHashedTokenWithoutPlus;
        }

        public string GenerateJWToken(User user, string JWTokenSigningKey)
        {
            // authentication successful so generate jwt token
            if (user is null || user is default(User))
            {
                throw new ArgumentNullException("user can not be null");
            }
            if (JWTokenSigningKey is null || JWTokenSigningKey is default(string))
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
    }
}
