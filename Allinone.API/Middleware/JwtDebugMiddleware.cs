using Allinone.BLL;
using System.IdentityModel.Tokens.Jwt;

namespace Allinone.API.Middleware
{
    public class JwtDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtDebugMiddleware> _logger;

        public JwtDebugMiddleware(RequestDelegate next, ILogger<JwtDebugMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("==== Incoming Request ====");
            _logger.LogInformation($"Path: {context.Request.Path}");
            _logger.LogInformation($"Method: {context.Request.Method}");

            // Log headers
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                _logger.LogInformation($"Authorization Header Present: {authHeader.ToString().Substring(0, Math.Min(50, authHeader.ToString().Length))}...");

                // Decode and log JWT token content for debugging
                try
                {
                    var token = authHeader.ToString().Replace("Bearer ", "");
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);
                        _logger.LogInformation($"Token Issuer: {jwtToken.Issuer}");
                        _logger.LogInformation($"Token Audience: {string.Join(", ", jwtToken.Audiences)}");
                        _logger.LogInformation($"Token Expires: {jwtToken.ValidTo}");
                        _logger.LogInformation($"Token IsExpired: {jwtToken.ValidTo < DateTime.UtcNow}");
                        _logger.LogInformation("Token Claims:");
                        foreach (var claim in jwtToken.Claims)
                        {
                            _logger.LogInformation($"  - {claim.Type}: {claim.Value}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to decode token: {ex.Message}");
                }
            }
            else
            {
                _logger.LogWarning("Authorization Header: Not Present");
            }

            // Log authentication status AFTER authentication middleware runs
            await _next(context);

            if (context.User?.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("✓ User is authenticated.");
                _logger.LogInformation($"Auth Scheme: {context.User.Identity.AuthenticationType}");

                _logger.LogInformation("User Claims:");
                foreach (var claim in context.User.Claims)
                {
                    _logger.LogInformation($"  - {claim.Type}: {claim.Value}");
                }

                var memberId = context.User.FindFirst("MemberId")?.Value;
                if (!string.IsNullOrEmpty(memberId))
                {
                    BaseBLL.MemberId = Convert.ToInt32(memberId);
                }
            }
            else
            {
                _logger.LogWarning("✗ User is NOT authenticated. Response Status: {StatusCode}", context.Response.StatusCode);
            }

            _logger.LogInformation("==========================");
        }
    }
}
