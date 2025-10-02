using FluentValidation;
using JobTracker.Application.DTOs;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Application.Common.Exceptions;

namespace JobTracker.Infrastructure.Services
{
    public class JobApplicationService(IJobApplicationRepository repository, IValidator<JobApplicationCreateDto> createValidator, IValidator<JobApplicationUpdateDto> updateValidator) : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository = repository;
        private readonly IValidator<JobApplicationCreateDto> _createValidator = createValidator;
        private readonly IValidator<JobApplicationUpdateDto> _updateValidator = updateValidator;

		public async Task<JobApplicationResponseDto> CreateAsync(Guid userId, JobApplicationCreateDto dto)
        {
            var result = await _createValidator.ValidateAsync(dto);

            if (!result.IsValid)
                throw new JobValidationException(result.Errors.Select(e => e.ErrorMessage));

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

            if (dto.CurrentStage.HasValue)
                application.ChangeStage(dto.CurrentStage.Value);

            if (dto.Status.HasValue)
                application.ChangeStatus(dto.Status.Value);

            await _repository.AddAsync(application);

            return ToResponseDto(application);
        }

        public async Task<List<JobApplicationResponseDto>> GetAllByUserAsync(Guid userId)
        {
            var list = await _repository.GetAllByUserAsync(userId);
            return [.. list.Select(ToResponseDto)];
        }

        public async Task<JobApplicationResponseDto?> GetByIdAsync(Guid id)
        {
            var app = await _repository.GetByIdAsync(id);
            return app is null ? null : ToResponseDto(app);
        }

        public async Task UpdateAsync(Guid id, JobApplicationUpdateDto dto)
        {
            var result = await _updateValidator.ValidateAsync(dto);

            if (!result.IsValid)
                throw new JobValidationException(result.Errors.Select(e => e.ErrorMessage));

            var app = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Application not found");

            app.UpdateFromDto(
                dto.CompanyName,
                dto.JobTitle,
                dto.SalaryExpectation,
                dto.JobOfferUrl,
                dto.Source,
                dto.Notes
            );

            if (dto.CurrentStage.HasValue)
                app.ChangeStage(dto.CurrentStage.Value);

            if (dto.Status.HasValue)
                app.ChangeStatus(dto.Status.Value);

            await _repository.UpdateAsync(app);
        }

        public async Task DeleteAsync(Guid id)
        {
            var app = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Application not found");
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
