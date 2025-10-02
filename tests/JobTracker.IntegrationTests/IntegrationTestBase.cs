using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentValidation;
using JobTracker.Api;
using JobTracker.Application.Common.Validators;
using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Infrastructure.Persistence;
using JobTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.IntegrationTests;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        // Override the factory for integration tests
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTest");
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<JobTrackerDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Add in-memory DbContext
                services.AddDbContext<JobTrackerDbContext>(options =>
                    options.UseInMemoryDatabase("IntegrationTestDb"));

                // Register JobApplicationService with validators
                services.AddScoped<IJobApplicationService, JobApplicationService>();
                services.AddScoped<IValidator<JobApplicationCreateDto>, JobApplicationCreateValidator>();
                services.AddScoped<IValidator<JobApplicationUpdateDto>, JobApplicationUpdateValidator>();
            });
        });

        _client = _factory.CreateClient();
    }

    #region Database Helpers

    protected async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<JobTrackerDbContext>();

        context.Users.RemoveRange(context.Users);
        context.JobApplications.RemoveRange(context.JobApplications);
        await context.SaveChangesAsync();
    }

    #endregion

    #region Auth Helpers

    protected async Task RegisterUserAsync(string email, string password)
    {
        var registerDto = new UserRegisterDto { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        response.EnsureSuccessStatusCode();
    }

    protected async Task<string> LoginAndGetTokenAsync(string email, string password)
    {
        var loginDto = new UserLoginDto { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>()
                     ?? throw new InvalidOperationException("Login response was null");
        return result.Token;
    }

    protected void AuthenticateClient(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    #endregion
}
