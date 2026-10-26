using Allinone.API.Services;
using Allinone.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Allinone.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IIdleTimeTrackingService _idleTimeService;
        private readonly JwtSettings _jwtSettings;

        public SessionController(IIdleTimeTrackingService idleTimeService, IOptions<JwtSettings> jwtOptions)
        {
            _idleTimeService = idleTimeService;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// Check current session status and remaining idle time
        /// </summary>
        /// <returns>Session information including last activity and remaining time</returns>
        [HttpGet("status")]
        public async Task<IActionResult> GetSessionStatus()
        {
            var userId = User.FindFirst("MemberId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ApiResponse(null)
                {
                    Success = false,
                    Message = "User ID not found in token"
                });
            }

            var lastActivity = await _idleTimeService.GetLastActivityAsync(userId);
            var idleTimeoutMinutes = _jwtSettings.IdleTimeoutMinutes;
            var idleTimeout = TimeSpan.FromMinutes(idleTimeoutMinutes);

            if (!lastActivity.HasValue)
            {
                return Ok(new ApiResponse(new
                {
                    userId = userId,
                    lastActivity = (DateTime?)null,
                    isActive = true,
                    message = "First request - activity tracking started"
                }));
            }

            var timeSinceLastActivity = DateTime.UtcNow - lastActivity.Value;
            var isActive = timeSinceLastActivity <= idleTimeout;
            var remainingTime = isActive ? idleTimeout - timeSinceLastActivity : TimeSpan.Zero;

            return Ok(new ApiResponse(new
            {
                userId = userId,
                lastActivity = lastActivity.Value,
                timeSinceLastActivity = timeSinceLastActivity,
                idleTimeoutMinutes = idleTimeoutMinutes,
                isActive = isActive,
                remainingTime = remainingTime,
                message = isActive ? "Session is active" : "Session expired due to inactivity"
            }));
        }

        /// <summary>
        /// Manually refresh the user's activity timestamp
        /// </summary>
        /// <returns>Confirmation of activity update</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshActivity()
        {
            var userId = User.FindFirst("MemberId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ApiResponse(null)
                {
                    Success = false,
                    Message = "User ID not found in token"
                });
            }

            await _idleTimeService.UpdateLastActivityAsync(userId);

            return Ok(new ApiResponse(new
            {
                userId = userId,
                lastActivity = DateTime.UtcNow,
                message = "Activity refreshed successfully"
            }));
        }
    }
}