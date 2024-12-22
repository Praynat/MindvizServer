
using Microsoft.EntityFrameworkCore;
using MindvizServer.Infrastructure.Data;
using MindvizServer.Application.Filters;
using MindvizServer.Infrastructure.Data;
using MindvizServer.Application.Interfaces;
using MindvizServer.Application.Services;
using MindvizServer.Core.Interfaces;
using MindvizServer.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace MindvizServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Part 1: Controller setup
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<LogActionFilter>(); // Add logging filter
                options.Filters.Add<ValidationModelFilter>(); // Add model validation filter
            })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }); ;


            // Part 2: Database context configuration
            var connectionString = Environment.GetEnvironmentVariable("MindvizDatabase");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string not found in environment variables.");
            }
            builder.Services.AddDbContext<MindvizDbContext>(options =>
                options.UseSqlServer(connectionString));



            // Part 3: Dependency Injection for services
            builder.Services.AddSingleton<IAuth, JwtAuthService>(); // Singleton authentication service
            builder.Services.AddScoped<IUserRepository, UserRepository>(); // Scoped user repository
            builder.Services.AddScoped<IUserService, UserService>(); // Scoped user service
            builder.Services.AddScoped<ITaskRepository, TaskRepository>(); // Scoped card repository
            builder.Services.AddScoped<ITaskService, TaskService>(); // Scoped card service

            // Part 4: Authentication setup
            var jwtKey = Environment.GetEnvironmentVariable("JwtSecretKey");
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT secret key not found in environment variables.");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // Validate the issuer
                        ValidateAudience = false, // Validate the audience
                        ValidateLifetime = true, // Ensure token expiry is checked
                        ValidIssuer = "MindvizServer", // Issuer value
                        ValidAudience = "MindvizWebApp", // Audience value
                        ValidateIssuerSigningKey = true, // Verify the signing key
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            // Part 5: Authorization setup
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Must be logged", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Normal") || context.User.IsInRole("Business") || context.User.IsInRole("Admin")));
                options.AddPolicy("Must be business", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Business") || context.User.IsInRole("Admin")));
                options.AddPolicy("Must be admin", policy => policy.RequireRole("Admin"));
            });

            // Part 6: CORS (Cross-Origin Resource Sharing) setup
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policyBuilder =>
                {
                    policyBuilder.AllowAnyOrigin() // Allow requests from any origin
                                 .AllowAnyMethod() // Allow all HTTP methods
                                 .AllowAnyHeader(); // Allow all headers
                });
            });


            // Part 7: API Explorer and Swagger (API documentation) setup
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Build the application
            var app = builder.Build();

            // Part 8: Middleware pipeline configuration
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // Enable Swagger in development mode
                app.UseSwaggerUI(); // Provide UI for Swagger
            }

            app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
            app.UseCors(); // Apply CORS policies
            app.UseAuthentication(); // Add authentication middleware
            app.UseAuthorization(); // Add authorization middleware

            // Part 9: Map controller routes
            app.MapControllers();

            // Start the application
            app.Run();
        }
    }
}
