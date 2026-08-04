using Scalar.AspNetCore;
using Serilog;
using Serilog.Context;
using System.Security.Claims;

namespace EventHub.API.Extensions
{
    public static class ApiMiddlewares
    {
        public static WebApplication UseApiMiddelwares(this WebApplication app)
        {
            
            app.UseExceptionHandler();

            // HTTPS Redirection
            app.UseHttpsRedirection();

            // Routing 
            app.UseRouting();

            // Developer Tools / Scalar OpenAPI
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "EventHub API";
                    options.Theme = ScalarTheme.Purple;
                });
            }

            // Authentication & Authorization 
            app.UseAuthentication();
            app.UseAuthorization();

           
            app.Use(async (context, next) =>
            {
                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? context.User?.FindFirst("sub")?.Value
                             ?? "Anonymous";

                using (LogContext.PushProperty("UserId", userId))
                {
                    await next();
                }
            });

            // Serilog HTTP Request Logging 
            app.UseSerilogRequestLogging();

            return app;
        }
    }
}