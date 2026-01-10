using Allinone.API.Services;
using System.Security.Claims;

namespace Allinone.API.Middleware
{
    public class ActivityTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IIdleTimeTrackingService _idleTimeService;

        public ActivityTrackingMiddleware(RequestDelegate next, IIdleTimeTrackingService idleTimeService)
        {
            _next = next;
            _idleTimeService = idleTimeService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Track activity for authenticated users
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst("MemberId")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await _idleTimeService.UpdateLastActivityAsync(userId);
                }
            }

            await _next(context);
        }
    }
}