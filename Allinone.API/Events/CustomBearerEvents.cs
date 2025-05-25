using Allinone.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Allinone.API.Events
{
    public class CustomBearerEvents : JwtBearerEvents
    {
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
    }
}
