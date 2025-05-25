using Allinone.BLL;

namespace Allinone.API.Middleware
{
    public class JwtDebugMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtDebugMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("==== Incoming Request ====");
            Console.WriteLine($"Path: {context.Request.Path}");
            Console.WriteLine($"Method: {context.Request.Method}");

            // Log headers
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                Console.WriteLine($"Authorization Header: {authHeader}");
            }
            else
            {
                Console.WriteLine("Authorization Header: Not Present");
            }

            // Log authentication status
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("User is authenticated.");
                Console.WriteLine($"Auth Scheme: {context.User.Identity.AuthenticationType}");

                Console.WriteLine("Claims:");
                foreach (var claim in context.User.Claims)
                {
                    Console.WriteLine($"- {claim.Type}: {claim.Value}");
                }

                var memberId = context.User.FindFirst("MemberId")?.Value;
                BaseBLL.MemberId = Convert.ToInt32(memberId);
            }
            else
            {
                Console.WriteLine("User is NOT authenticated.");
            }

            Console.WriteLine("==========================");

            await _next(context);
        }
    }
}
