using Allinone.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Allinone.Helper.JWT
{
    public record JwtTokenResult(string Token, string TokenIdentifier, DateTime ExpiresAt);

    public static class JWTHelper
    {
        public const string MemberIdClaim = "MemberId";

        public static JwtTokenResult GenerateJwtToken(string username, int memberId, string role, JwtSettings settings)
        {
            var jti = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(MemberIdClaim, memberId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(settings.ExpireMinutes);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), jti, expiresAt);
        }
    }
}
