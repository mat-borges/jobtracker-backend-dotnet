using JobTracker.Api;
using JobTracker.Application.DTOs;
using JobTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace JobTracker.IntegrationTests.Api
{
    public class JobApplicationControllerTests(WebApplicationFactory<Program> factory)
        : IntegrationTestBase(factory)
    {
        private readonly string _email = "testuser@example.com";
        private readonly string _password = "StrongPass123!";

        // Setup inicial: registra e autentica usuário
        private async Task AuthenticateAsync()
        {
            await RegisterUserAsync(_email, _password);
            var token = await LoginAndGetTokenAsync(_email, _password);
            AuthenticateClient(token);
        }

        [Fact]
        public async Task CreateJobApplication_ShouldReturnCreated()
        {
            await AuthenticateAsync();

            var newJobApp = new JobApplicationCreateDto
            {
                JobTitle = "Backend Developer",
                CompanyName = "Tech Corp",
                ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ContractType = ContractType.CLT,
                WorkStyle = WorkStyle.Remote
            };

            var response = await _client.PostAsJsonAsync("/api/jobapplications", newJobApp);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<JobApplicationResponseDto>();
            Assert.NotNull(created);
            Assert.Equal(newJobApp.JobTitle, created.JobTitle);
            Assert.Equal(newJobApp.CompanyName, created.CompanyName);
        }

        [Fact]
        public async Task GetAllJobApplications_ShouldReturnList()
        {
            await AuthenticateAsync();

            // Criar uma aplicação para garantir que há dados
            var newJobApp = new JobApplicationCreateDto
            {
                JobTitle = "Frontend Developer",
                CompanyName = "Web Corp",
                ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ContractType = ContractType.CLT,
                WorkStyle = WorkStyle.Hybrid
            };
            await _client.PostAsJsonAsync("/api/jobapplications", newJobApp);

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

            var newJobApp = new JobApplicationCreateDto
            {
                JobTitle = "QA Engineer",
                CompanyName = "Test Corp",
                ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ContractType = ContractType.CLT,
                WorkStyle = WorkStyle.Local
            };
            var createResponse = await _client.PostAsJsonAsync("/api/jobapplications", newJobApp);
            var created = await createResponse.Content.ReadFromJsonAsync<JobApplicationResponseDto>();

            var response = await _client.GetAsync($"/api/jobapplications/{created.Id}");
            response.EnsureSuccessStatusCode();

            var fetched = await response.Content.ReadFromJsonAsync<JobApplicationResponseDto>();
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
            Assert.Equal(newJobApp.JobTitle, fetched.JobTitle);
        }

        [Fact]
        public async Task UpdateJobApplication_ShouldReturnNoContent()
        {
            await AuthenticateAsync();

            var newJobApp = new JobApplicationCreateDto
            {
                JobTitle = "DevOps Engineer",
                CompanyName = "Ops Corp",
                ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ContractType = ContractType.CLT,
                WorkStyle = WorkStyle.Remote
            };
            var createResponse = await _client.PostAsJsonAsync("/api/jobapplications", newJobApp);
            var created = await createResponse.Content.ReadFromJsonAsync<JobApplicationResponseDto>();

            var updateDto = new JobApplicationUpdateDto
            {
                JobTitle = "Senior DevOps Engineer",
                CompanyName = "Ops Corp Updated",
                SalaryExpectation = 120000m,
                JobOfferUrl = "http://example.com/job",
                Source = "LinkedIn",
                Notes = "Updated notes"
            };

            var response = await _client.PutAsJsonAsync($"/api/jobapplications/{created.Id}", updateDto);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/jobapplications/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<JobApplicationResponseDto>();
            Assert.Equal(updateDto.JobTitle, updated.JobTitle);
            Assert.Equal(updateDto.CompanyName, updated.CompanyName);
            Assert.Equal(updateDto.Notes, updated.Notes);
        }

        [Fact]
        public async Task DeleteJobApplication_ShouldReturnNoContent()
        {
            await AuthenticateAsync();

            var newJobApp = new JobApplicationCreateDto
            {
                JobTitle = "Product Manager",
                CompanyName = "Product Corp",
                ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ContractType = ContractType.CLT,
                WorkStyle = WorkStyle.Hybrid
            };
            var createResponse = await _client.PostAsJsonAsync("/api/jobapplications", newJobApp);
            var created = await createResponse.Content.ReadFromJsonAsync<JobApplicationResponseDto>();

            var response = await _client.DeleteAsync($"/api/jobapplications/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/jobapplications/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
