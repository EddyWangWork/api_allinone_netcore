using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Allinone.Helper.JWT
{
    public static class JWTHelper
    {
        public static string GenerateJwtToken(string username, int memberId, IConfiguration? configuration = null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("MemberId", memberId.ToString())
            };

            // Use configuration values if available, otherwise use defaults
            var key = configuration?["Jwt:Key"] ?? "YourSuperSecretKeyHere123!";
            var issuer = configuration?["Jwt:Issuer"] ?? "ProductServiceApp";
            var audience = configuration?["Jwt:Audience"] ?? "ProductServiceUsers";
            var expireMinutes = 60;
            if (configuration != null && int.TryParse(configuration["Jwt:ExpireMinutes"], out var configMinutes))
            {
                expireMinutes = configMinutes;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
