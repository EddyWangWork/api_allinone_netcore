using Allinone.API.Services;
using Allinone.DLL.Repositories;
using Allinone.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace Allinone.API.Events
{
    public class CustomBearerEvents : JwtBearerEvents
    {
        private readonly IIdleTimeTrackingService _idleTimeService;
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
        private readonly IConfiguration _configuration;

        public CustomBearerEvents(
            IIdleTimeTrackingService idleTimeService,
            ITokenBlacklistRepository tokenBlacklistRepository,
            IConfiguration configuration)
        {
            _idleTimeService = idleTimeService;
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _configuration = configuration;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                var apiResponse = new ApiResponse(null)
                {
                    Success = false,
                    Message = "Token-Expired"
                };

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(apiResponse);
                return context.Response.WriteAsync(json);
            }

            return Task.CompletedTask;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            // Extract token identifier (JTI) from the JWT
            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            // Check if token is blacklisted
            if (!string.IsNullOrEmpty(jti) && await _tokenBlacklistRepository.IsTokenBlacklistedAsync(jti))
            {
                context.Fail("Token has been revoked");

                var apiResponse = new ApiResponse(null)
                {
                    Success = false,
                    Message = "Token-Revoked"
                };

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(apiResponse);
                await context.Response.WriteAsync(json);
                return;
            }

            // Get user ID from claims
            var userId = context.Principal?.FindFirst("MemberId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Get idle timeout from configuration (in minutes, default to 30)
                var idleTimeoutMinutes = _configuration.GetValue<int>("Jwt:IdleTimeoutMinutes", 30);
                var idleTimeout = TimeSpan.FromMinutes(idleTimeoutMinutes);

                // Check if user has been idle for too long
                if (!await _idleTimeService.IsUserActiveAsync(userId, idleTimeout))
                {
                    var lastActivity = await _idleTimeService.GetLastActivityAsync(userId);

                    // If user has no recorded activity, allow them through (first request)
                    // Otherwise, if they've been idle too long, mark as failed
                    if (lastActivity.HasValue)
                    {
                        context.Fail("User session expired due to inactivity");

                        var apiResponse = new ApiResponse(null)
                        {
                            Success = false,
                            Message = "Session-Expired-Idle"
                        };

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var json = JsonSerializer.Serialize(apiResponse);
                        await context.Response.WriteAsync(json);
                        return;
                    }
                }

                // Update last activity time for active users
                await _idleTimeService.UpdateLastActivityAsync(userId);
            }
        }

        public override Task MessageReceived(MessageReceivedContext context)
        {
            // This runs before token validation - we can extract user info here if needed
            return Task.CompletedTask;
        }
    }
}
