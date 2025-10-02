using JobTracker.Api;
using JobTracker.Application.DTOs;
using JobTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
namespace JobTracker.IntegrationTests.Api;

public class JobApplicationControllerTests(WebApplicationFactory<Program> factory) : IntegrationTestBase(factory)
{
    private readonly string _email = "testuser@example.com";
    private readonly string _password = "StrongPass123!";

	#region Helpers

	private async Task AuthenticateAsync()
    {
        await ResetDatabaseAsync();
        await RegisterUserAsync(_email, _password);
        var token = await LoginAndGetTokenAsync(_email, _password);
        AuthenticateClient(token);
    }
    private async Task<JobApplicationResponseDto> CreateSampleJobApplicationAsync(
        string title = "Backend Developer",
        string company = "Tech Corp",
        ApplicationStage? stage = null,
        ApplicationStatus? status = null)
    {
        var dto = new JobApplicationCreateDto
        {
            JobTitle = title,
            CompanyName = company,
            ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ContractType = ContractType.CLT,
            WorkStyle = WorkStyle.Remote,
            WorkLocationState = "SP",
            WorkLocationCity = "São Paulo",
            WorkLocationCountry = "Brazil",
            JobOfferUrl = "http://example.com/job",
            Source = "LinkedIn",
            Notes = "Exciting opportunity",
            SalaryExpectation = 2000m,
            CurrentStage = stage,
            Status = status
        };
        var response = await _client.PostAsJsonAsync("/api/jobapplications", dto);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JobApplicationResponseDto>()
               ?? throw new InvalidOperationException("Failed to deserialize JobApplicationResponseDto.");
    }

    #endregion

    #region Success Scenarios

    [Fact]
    public async Task CreateJobApplication_ShouldReturnCreated()
    {
        await AuthenticateAsync();
        var created = await CreateSampleJobApplicationAsync(
            stage: ApplicationStage.Applied,
            status: ApplicationStatus.InProgress);

        Assert.NotNull(created);
        Assert.Equal("Backend Developer", created.JobTitle);
        Assert.Equal("Tech Corp", created.CompanyName);
        Assert.Equal(ApplicationStage.Applied, created.CurrentStage);
        Assert.Equal(ApplicationStatus.InProgress, created.Status);
    }

    [Fact]
    public async Task GetAllJobApplications_ShouldReturnList()
    {
        await AuthenticateAsync();
        await CreateSampleJobApplicationAsync(title: "Frontend Developer", company: "Web Corp");

        var response = await _client.GetAsync("/api/jobapplications");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<JobApplicationResponseDto>>();

        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task GetJobApplicationById_ShouldReturnApplication()
    {
        await AuthenticateAsync();
        var created = await CreateSampleJobApplicationAsync(title: "QA Engineer", company: "Test Corp");

        var response = await _client.GetAsync($"/api/jobapplications/{created.Id}");
        response.EnsureSuccessStatusCode();
        var fetched = await response.Content.ReadFromJsonAsync<JobApplicationResponseDto>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal("QA Engineer", fetched.JobTitle);
    }

    [Fact]
    public async Task UpdateJobApplication_ShouldReturnNoContent()
    {
        await AuthenticateAsync();
        var created = await CreateSampleJobApplicationAsync(title: "DevOps Engineer", company: "Ops Corp");

        var updateDto = new JobApplicationUpdateDto
        {
            JobTitle = "Senior DevOps Engineer",
            CompanyName = "Ops Corp Updated",
            SalaryExpectation = 120000m,
            JobOfferUrl = "http://example.com/job",
            Source = "LinkedIn",
            Notes = "Updated notes",
            CurrentStage = ApplicationStage.RHInterview,
            Status = ApplicationStatus.InProgress
        };

        var response = await _client.PutAsJsonAsync($"/api/jobapplications/{created.Id}", updateDto);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/jobapplications/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<JobApplicationResponseDto>();
        Assert.Equal(updateDto.JobTitle, updated!.JobTitle);
        Assert.Equal(updateDto.CompanyName, updated.CompanyName);
        Assert.Equal(updateDto.Notes, updated.Notes);
        Assert.Equal(ApplicationStage.RHInterview, updated.CurrentStage);
        Assert.Equal(ApplicationStatus.InProgress, updated.Status);
    }

    [Fact]
    public async Task DeleteJobApplication_ShouldReturnNoContent()
    {
        await AuthenticateAsync();
        var created = await CreateSampleJobApplicationAsync(title: "Product Manager", company: "Product Corp");

        var response = await _client.DeleteAsync($"/api/jobapplications/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/jobapplications/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    #endregion

    #region Failure / Validation Scenarios

    [Fact]
    public async Task CreateJobApplication_InvalidData_ShouldReturnBadRequest()
    {
        await AuthenticateAsync();
        var invalidDto = new JobApplicationCreateDto
        {
            JobTitle = "",
            CompanyName = "",
            ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            ContractType = ContractType.CLT,
            WorkStyle = WorkStyle.Remote
        };

        var response = await _client.PostAsJsonAsync("/api/jobapplications", invalidDto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateJobApplication_NonExistentId_ShouldReturnNotFound()
    {
        await AuthenticateAsync();
        Console.WriteLine("Creating update DTO for non-existent job application.");
        var updateDto = new JobApplicationUpdateDto
        {
            JobTitle = "Non-existent Job",
            CompanyName = "Nowhere",
            CurrentStage = ApplicationStage.Applied,
            Status = ApplicationStatus.InProgress
        };


        var response = await _client.PutAsJsonAsync($"/api/jobapplications/{Guid.NewGuid()}", updateDto);
        Console.WriteLine($"Update response status: {response.StatusCode}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteJobApplication_NonExistentId_ShouldReturnNotFound()
    {
        await AuthenticateAsync();
        var response = await _client.DeleteAsync($"/api/jobapplications/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetJobApplicationById_NonExistentId_ShouldReturnNotFound()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync($"/api/jobapplications/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AccessEndpoints_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var responseGet = await _client.GetAsync("/api/jobapplications");
        Assert.Equal(HttpStatusCode.Unauthorized, responseGet.StatusCode);

        var responsePost = await _client.PostAsJsonAsync("/api/jobapplications", new JobApplicationCreateDto
        {
            JobTitle = "Job",
            CompanyName = "Company",
            ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ContractType = ContractType.CLT,
            WorkStyle = WorkStyle.Remote
        });
        Assert.Equal(HttpStatusCode.Unauthorized, responsePost.StatusCode);
    }

    #endregion
}
