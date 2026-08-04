using EventHub.API.Middlewares;
using EventHub.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace EventHub.API.Extensions
{
    public static class DependencyInjectionAPI
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration,IHostBuilder host)
        {
            // Serilog Logging
            services.AddSerilogLogging(configuration, host);

            // Http Context Accessor
            services.AddHttpContextAccessor();

            // Exception Handling
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // 3. Authentication & Authorization
            services.AddJwtAuthentication(configuration);
            services.AddAuthorization();

            // OpenAPI / Swagger Documentation
            services.AddOpenApiDocumentation();

            services.AddMemoryCache();

            return services;
        }

        private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            var logTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [User: {UserId}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: logTemplate)
                .WriteTo.File(
                    path: "Logs/eventhub-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: logTemplate)
                .CreateLogger();

            host.UseSerilog();

            return services;
        }
        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                };
            });

            return services;
        }

        private static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();

                    document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Insert Token Here"
                    });

                    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}