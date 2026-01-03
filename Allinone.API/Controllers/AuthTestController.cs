using Allinone.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Allinone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthTestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Test endpoint that doesn't require authentication
        /// </summary>
        [AllowAnonymous]
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok(new ApiResponse(new
            {
                message = "This endpoint is public and doesn't require authentication",
                timestamp = DateTime.UtcNow
            }));
        }

        /// <summary>
        /// Test endpoint that requires authentication
        /// </summary>
        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            var userId = User.FindFirst("MemberId")?.Value;
            var username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return Ok(new ApiResponse(new
            {
                message = "You are authenticated!",
                userId = userId,
                username = username,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                timestamp = DateTime.UtcNow
            }));
        }

        /// <summary>
        /// Decode a JWT token without validating it (for debugging)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("decode-token")]
        public IActionResult DecodeToken([FromBody] TokenRequest request)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(request.Token))
                {
                    return BadRequest(new ApiResponse(null)
                    {
                        Success = false,
                        Message = "Invalid token format"
                    });
                }

                var jwtToken = handler.ReadJwtToken(request.Token);

                var expectedIssuer = _configuration["Jwt:Issuer"];
                var expectedAudience = _configuration["Jwt:Audience"];

                return Ok(new ApiResponse(new
                {
                    issuer = jwtToken.Issuer,
                    audiences = jwtToken.Audiences.ToList(),
                    expires = jwtToken.ValidTo,
                    isExpired = jwtToken.ValidTo < DateTime.UtcNow,
                    claims = jwtToken.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                    expectedConfiguration = new
                    {
                        issuer = expectedIssuer,
                        audience = expectedAudience
                    },
                    matches = new
                    {
                        issuerMatches = jwtToken.Issuer == expectedIssuer,
                        audienceMatches = jwtToken.Audiences.Contains(expectedAudience)
                    }
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(null)
                {
                    Success = false,
                    Message = $"Failed to decode token: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Show current JWT configuration
        /// </summary>
        [AllowAnonymous]
        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(new ApiResponse(new
            {
                issuer = _configuration["Jwt:Issuer"],
                audience = _configuration["Jwt:Audience"],
                expireMinutes = _configuration.GetValue<int>("Jwt:ExpireMinutes"),
                idleTimeoutMinutes = _configuration.GetValue<int>("Jwt:IdleTimeoutMinutes"),
                note = "The 'Key' is hidden for security reasons"
            }));
        }
    }

    public class TokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}