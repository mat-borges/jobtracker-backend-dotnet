using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;

namespace JobTracker.Infrastructure.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;

        public JobApplicationService(IJobApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<JobApplicationResponseDto> CreateAsync(Guid userId, JobApplicationCreateDto dto)
        {
            var application = new JobApplication(
                userId,
                dto.ApplicationDate,
                dto.CompanyName,
                dto.JobTitle,
                dto.ContractType,
                dto.WorkStyle,
                dto.WorkLocationState
            );

            application.UpdateFromDto(
                dto.CompanyName,
                dto.JobTitle,
                dto.SalaryExpectation,
                dto.JobOfferUrl,
                dto.Source,
                dto.Notes
            );

            await _repository.AddAsync(application);

            return ToResponseDto(application);
        }

        public async Task<List<JobApplicationResponseDto>> GetAllByUserAsync(Guid userId)
        {
            var list = await _repository.GetAllByUserAsync(userId);
            return list.Select(ToResponseDto).ToList();
        }

        public async Task<JobApplicationResponseDto?> GetByIdAsync(Guid id)
        {
            var app = await _repository.GetByIdAsync(id);
            return app is null ? null : ToResponseDto(app);
        }

        public async Task UpdateAsync(Guid id, JobApplicationUpdateDto dto)
        {
            var app = await _repository.GetByIdAsync(id) ?? throw new Exception("Application not found");
			app.UpdateFromDto(dto.CompanyName, dto.JobTitle, dto.SalaryExpectation, dto.JobOfferUrl, dto.Source, dto.Notes);

            await _repository.UpdateAsync(app);
        }

        public async Task DeleteAsync(Guid id)
        {
            var app = await _repository.GetByIdAsync(id) ?? throw new Exception("Application not found");
			await _repository.DeleteAsync(app);
        }

        private JobApplicationResponseDto ToResponseDto(JobApplication a) =>
            new()
            {
                Id = a.Id,
                ApplicationDate = a.ApplicationDate,
                CompanyName = a.CompanyName,
                JobTitle = a.JobTitle,
                ContractType = a.ContractType,
                WorkStyle = a.WorkStyle,
                WorkLocationCity = a.WorkLocationCity,
                WorkLocationState = a.WorkLocationState,
                WorkLocationCountry = a.WorkLocationCountry,
                JobOfferUrl = a.JobOfferUrl,
                CurrentStage = a.CurrentStage,
                Status = a.Status,
                SalaryExpectation = a.SalaryExpectation,
                Source = a.Source,
                Notes = a.Notes,
                ProcessDays = a.ProcessDays
            };
    }
}
