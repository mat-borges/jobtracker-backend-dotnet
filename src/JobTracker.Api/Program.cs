using System.Text;
using FluentValidation;
using JobTracker.Api.Middleware;
using JobTracker.Application.Common.Validators;
using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Infrastructure.Persistence;
using JobTracker.Infrastructure.Repositories;
using JobTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobTracker.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        // ======================
        // DbContext
        // ======================
        builder.Services.AddDbContext<JobTrackerDbContext>(options =>
        {
            if (environment.EnvironmentName == "IntegrationTest")
                options.UseInMemoryDatabase("IntegrationTestDb");
            else
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        // ======================
        // Repositories & Services
        // ======================
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

        builder.Services.AddScoped<IJobApplicationService, JobApplicationService>(sp =>
        {
            var repo = sp.GetRequiredService<IJobApplicationRepository>();
            var createValidator = sp.GetRequiredService<IValidator<JobApplicationCreateDto>>();
            var updateValidator = sp.GetRequiredService<IValidator<JobApplicationUpdateDto>>();
            return new JobApplicationService(repo, createValidator, updateValidator);
        });

        // ======================
        // Controllers & Validation
        // ======================
        builder.Services.AddControllers();
        builder.Services.AddValidatorsFromAssemblyContaining<JobApplicationCreateValidator>();

        // ======================
        // Swagger
        // ======================
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // ======================
        // CORS
        // ======================
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevCors", cors =>
                cors.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());

            options.AddPolicy("ProdCors", cors =>
                cors.WithOrigins("https://myProductionSite.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        // ======================
        // JWT Authentication
        // ======================
        var jwtSection = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

        builder.Services.AddAuthentication(options =>
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
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("DevCors");
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseCors("ProdCors");
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
