using Allinone.API.Services;
using Allinone.DLL.Repositories;
using Allinone.Domain;
using Allinone.Domain.Members;
using Allinone.Domain.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Allinone.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    //[Route("api/[controller]")]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
        private readonly IIdleTimeTrackingService _idleTimeService;
        private readonly IConfiguration _configuration;

        public AdminController(
            IMemberRepository memberRepository,
            IUserSessionRepository sessionRepository,
            ITokenBlacklistRepository tokenBlacklistRepository,
            IIdleTimeTrackingService idleTimeService,
            IConfiguration configuration)
        {
            _memberRepository = memberRepository;
            _sessionRepository = sessionRepository;
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _idleTimeService = idleTimeService;
            _configuration = configuration;
        }

        /// <summary>
        /// Create a new member (admin only)
        /// </summary>
        [HttpPost("members")]
        public async Task<IActionResult> CreateMember([FromBody] CreateMemberRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new ApiResponse(null)
                {
                    Success = false,
                    Message = "Username is required"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ApiResponse(null)
                {
                    Success = false,
                    Message = "Password is required"
                });
            }

            // Validate role
            var validRoles = new[] { "Admin", "User" };
            if (!validRoles.Contains(request.Role))
            {
                return BadRequest(new ApiResponse(null)
                {
                    Success = false,
                    Message = $"Invalid role. Must be 'Admin' or 'User'"
                });
            }

            // Check if user already exists
            var existingMembers = await _memberRepository.GetAllAsync();
            if (existingMembers.Any(m => m.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new ApiResponse(null)
                {
                    Success = false,
                    Message = "A user with this name already exists"
                });
            }

            // Create new member
            var newMember = new Member
            {
                Name = request.Name,
                Password = request.Password,
                Role = request.Role,
                IsActive = request.IsActive
            };

            await _memberRepository.Add(newMember);

            return CreatedAtAction(
                nameof(GetMember),
                new { id = newMember.ID },
                new
                {
                    id = newMember.ID,
                    name = newMember.Name,
                    role = newMember.Role,
                    isActive = newMember.IsActive,
                    message = "Member created successfully"
                });
        }

        /// <summary>
        /// Get all members (admin only)
        /// </summary>
        [HttpGet("members")]
        public async Task<IActionResult> GetAllMembers()
        {
            var members = await _memberRepository.GetAllAsync();

            var memberList = members.Select(m => new
            {
                id = m.ID,
                name = m.Name,
                role = m.Role,
                isActive = m.IsActive,
                lastLoginDate = m.LastLoginDate
            });

            return Ok(memberList);
            //return Ok(new ApiResponse(memberList));
        }

        /// <summary>
        /// Get member by ID (admin only)
        /// </summary>
        [HttpGet("members/{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            var members = await _memberRepository.GetAllAsync();
            var member = members.FirstOrDefault(m => m.ID == id);

            if (member == null)
            {
                return NotFound(new ApiResponse(null)
                {
                    Success = false,
                    Message = "Member not found"
                });
            }

            var memberInfo = new
            {
                id = member.ID,
                name = member.Name,
                lastLoginDate = member.LastLoginDate
            };

            return Ok(memberInfo);
        }

        /// <summary>
        /// Delete member (admin only)
        /// </summary>
        [HttpDelete("members/{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var members = await _memberRepository.GetAllAsync();
            var member = members.FirstOrDefault(m => m.ID == id);

            if (member == null)
            {
                return NotFound(new ApiResponse(null)
                {
                    Success = false,
                    Message = "Member not found"
                });
            }

            // Delete user's session first
            await _sessionRepository.DeleteAsync(id);

            // Then delete the member
            await _memberRepository.Delete(id);

            return Ok(new
            {
                message = $"Member '{member.Name}' deleted successfully"
            });

            //return Ok(new ApiResponse(new
            //{
            //    message = $"Member '{member.Name}' deleted successfully"
            //}));
        }

        /// <summary>
        /// Get all active sessions (admin only)
        /// </summary>
        [HttpGet("sessions")]
        public async Task<IActionResult> GetAllSessions()
        {
            var members = await _memberRepository.GetAllAsync();
            var memberDict = members.ToDictionary(m => m.ID, m => m.Name);

            // Get sessions from repository - we need to implement GetAllAsync
            var sessionList = new List<object>();

            foreach (var member in members)
            {
                var session = await _sessionRepository.GetByMemberIdAsync(member.ID);
                if (session != null)
                {
                    var idleTimeoutMinutes = _configuration.GetValue<int>("Jwt:IdleTimeoutMinutes", 30);
                    var timeSinceLastActivity = DateTime.UtcNow - session.LastActivityTime;
                    var isActive = timeSinceLastActivity <= TimeSpan.FromMinutes(idleTimeoutMinutes);

                    sessionList.Add(new
                    {
                        memberId = session.MemberID,
                        memberName = memberDict.GetValueOrDefault(session.MemberID, "Unknown"),
                        lastActivityTime = session.LastActivityTime,
                        createdAt = session.CreatedAt,
                        expiresAt = session.ExpiresAt,
                        timeSinceLastActivity = timeSinceLastActivity,
                        isActive = isActive,
                        tokenIdentifier = session.TokenIdentifier,
                        ipAddress = session.IpAddress,
                        userAgent = session.UserAgent
                    });
                }
            }

            return Ok(new
            {
                totalSessions = sessionList.Count,
                activeSessions = sessionList.Count(s => ((dynamic)s).isActive),
                sessions = sessionList.OrderByDescending(s => ((dynamic)s).lastActivityTime)
            });
        }

        /// <summary>
        /// Get session for specific user (admin only)
        /// </summary>
        [HttpGet("sessions/{memberId}")]
        public async Task<IActionResult> GetUserSession(int memberId)
        {
            var session = await _sessionRepository.GetByMemberIdAsync(memberId);

            if (session == null)
            {
                return NotFound(new ApiResponse(null)
                {
                    Success = false,
                    Message = "No active session found for this user"
                });
            }

            var members = await _memberRepository.GetAllAsync();
            var member = members.FirstOrDefault(m => m.ID == memberId);

            var idleTimeoutMinutes = _configuration.GetValue<int>("Jwt:IdleTimeoutMinutes", 30);
            var timeSinceLastActivity = DateTime.UtcNow - session.LastActivityTime;
            var isActive = timeSinceLastActivity <= TimeSpan.FromMinutes(idleTimeoutMinutes);

            return Ok(new
            {
                memberId = session.MemberID,
                memberName = member?.Name ?? "Unknown",
                lastActivityTime = session.LastActivityTime,
                createdAt = session.CreatedAt,
                expiresAt = session.ExpiresAt,
                timeSinceLastActivity = timeSinceLastActivity,
                isActive = isActive,
                idleTimeoutMinutes = idleTimeoutMinutes,
                tokenIdentifier = session.TokenIdentifier,
                ipAddress = session.IpAddress,
                userAgent = session.UserAgent
            });
        }

        /// <summary>
        /// Force logout user by deleting their session (admin only)
        /// </summary>
        [HttpDelete("sessions/{memberId}")]
        public async Task<IActionResult> ForceLogoutUser(int memberId)
        {
            var session = await _sessionRepository.GetByMemberIdAsync(memberId);

            if (session == null)
            {
                return NotFound(new ApiResponse(null)
                {
                    Success = false,
                    Message = "No active session found for this user"
                });
            }

            var members = await _memberRepository.GetAllAsync();
            var member = members.FirstOrDefault(m => m.ID == memberId);

            // Add token to blacklist before deleting session
            if (!string.IsNullOrEmpty(session.TokenIdentifier))
            {
                var expireMinutes = _configuration.GetValue<int>("Jwt:ExpireMinutes", 60);
                var tokenExpiry = DateTime.UtcNow.AddMinutes(expireMinutes);

                await _tokenBlacklistRepository.AddToBlacklistAsync(
                    session.TokenIdentifier,
                    tokenExpiry,
                    "Admin force logout",
                    memberId
                );
            }

            // Delete session
            await _sessionRepository.DeleteAsync(memberId);

            return Ok(new
            {
                message = $"User '{member?.Name ?? memberId.ToString()}' has been logged out",
                memberId = memberId
            });
        }

        /// <summary>
        /// Get active users count (admin only)
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var members = await _memberRepository.GetAllAsync();
            var totalMembers = members.Count();

            var idleTimeoutMinutes = _configuration.GetValue<int>("Jwt:IdleTimeoutMinutes", 30);
            var activeSessions = 0;
            var idleSessions = 0;

            foreach (var member in members)
            {
                var session = await _sessionRepository.GetByMemberIdAsync(member.ID);
                if (session != null)
                {
                    var timeSinceLastActivity = DateTime.UtcNow - session.LastActivityTime;
                    if (timeSinceLastActivity <= TimeSpan.FromMinutes(idleTimeoutMinutes))
                    {
                        activeSessions++;
                    }
                    else
                    {
                        idleSessions++;
                    }
                }
            }

            return Ok(new
            {
                totalMembers = totalMembers,
                totalSessions = activeSessions + idleSessions,
                activeSessions = activeSessions,
                idleSessions = idleSessions,
                idleTimeoutMinutes = idleTimeoutMinutes
            });
        }

        /// <summary>
        /// Clean up all expired sessions (admin only)
        /// </summary>
        [HttpPost("sessions/cleanup")]
        public async Task<IActionResult> CleanupExpiredSessions()
        {
            await _sessionRepository.DeleteExpiredSessionsAsync();

            return Ok(new
            {
                message = "Expired sessions cleaned up successfully"
            });
        }

        /// <summary>
        /// Update member password (admin only)
        /// </summary>
        [HttpPut("members/{id}/password")]
        public async Task<IActionResult> UpdateMemberPassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new ApiResponse(null)
                {
                    Success = false,
                    Message = "New password is required"
                });
            }

            var members = await _memberRepository.GetAllAsync();
            var member = members.FirstOrDefault(m => m.ID == id);

            if (member == null)
            {
                return NotFound(new ApiResponse(null)
                {
                    Success = false,
                    Message = "Member not found"
                });
            }

            member.Password = request.NewPassword;
            _memberRepository.Update(member);

            // Optionally force logout after password change
            if (request.ForceLogout)
            {
                await _sessionRepository.DeleteAsync(id);
            }

            return Ok(new
            {
                message = $"Password updated successfully for user '{member.Name}'",
                forceLogout = request.ForceLogout
            });
        }

        /// <summary>
        /// Update member details (admin only)
        /// </summary>
        [HttpPut("members/{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] UpdateMemberRequest request)
        {
            var members = await _memberRepository.GetAllAsync();
            var member = members.FirstOrDefault(m => m.ID == id);

            if (member == null)
            {
                return NotFound(new ApiResponse(null)
                {
                    Success = false,
                    Message = "Member not found"
                });
            }

            // Check if new name conflicts with existing member (excluding current member)
            if (!string.IsNullOrWhiteSpace(request.Name) && !member.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (members.Any(m => m.ID != id && m.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return Conflict(new ApiResponse(null)
                    {
                        Success = false,
                        Message = "A user with this name already exists"
                    });
                }
                member.Name = request.Name;
            }

            // Validate role
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var validRoles = new[] { "Admin", "User" };
                if (!validRoles.Contains(request.Role))
                {
                    return BadRequest(new ApiResponse(null)
                    {
                        Success = false,
                        Message = $"Invalid role. Must be 'Admin' or 'User'"
                    });
                }
                member.Role = request.Role;
            }

            member.IsActive = request.IsActive;

            _memberRepository.Update(member);

            return Ok(new
            {
                message = $"Member '{member.Name}' updated successfully",
                member = new
                {
                    id = member.ID,
                    name = member.Name,
                    role = member.Role,
                    isActive = member.IsActive
                }
            });
        }
    }

    public class CreateMemberRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
    }

    public class UpdateMemberRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UpdatePasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
        public bool ForceLogout { get; set; } = true;
    }
}
