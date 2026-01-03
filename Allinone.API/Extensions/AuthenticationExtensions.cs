using Allinone.API.Configuration;
using Allinone.API.Events;
using Allinone.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Allinone.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCustomJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure JWT settings
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            var jwtSettings = new JwtSettings();
            configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };

                    // Configure events with dependency injection
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = async context =>
                        {
                            var serviceProvider = context.HttpContext.RequestServices;
                            var idleTimeService = serviceProvider.GetRequiredService<IIdleTimeTrackingService>();
                            var config = serviceProvider.GetRequiredService<IConfiguration>();
                            var customEvents = new CustomBearerEvents(idleTimeService, config);
                            await customEvents.AuthenticationFailed(context);
                        },
                        OnTokenValidated = async context =>
                        {
                            var serviceProvider = context.HttpContext.RequestServices;
                            var idleTimeService = serviceProvider.GetRequiredService<IIdleTimeTrackingService>();
                            var config = serviceProvider.GetRequiredService<IConfiguration>();
                            var customEvents = new CustomBearerEvents(idleTimeService, config);
                            await customEvents.TokenValidated(context);
                        },
                        OnMessageReceived = async context =>
                        {
                            var serviceProvider = context.HttpContext.RequestServices;
                            var idleTimeService = serviceProvider.GetRequiredService<IIdleTimeTrackingService>();
                            var config = serviceProvider.GetRequiredService<IConfiguration>();
                            var customEvents = new CustomBearerEvents(idleTimeService, config);
                            await customEvents.MessageReceived(context);
                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}