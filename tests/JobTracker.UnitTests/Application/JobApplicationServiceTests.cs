using Xunit;
using Moq;
using FluentValidation;
using FluentValidation.Results;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Services;
using JobTracker.Application.DTOs;
using JobTracker.Domain.Enums;

namespace JobTracker.UnitTests.Application
{
	public class JobApplicationServiceTests
	{
		private readonly IJobApplicationService _service;
		private readonly Mock<IJobApplicationRepository> _repositoryMock;
		private readonly Mock<IValidator<JobApplicationCreateDto>> _createValidatorMock;
		private readonly Mock<IValidator<JobApplicationUpdateDto>> _updateValidatorMock;

		public JobApplicationServiceTests()
		{
			_repositoryMock = new Mock<IJobApplicationRepository>();
			_createValidatorMock = new Mock<IValidator<JobApplicationCreateDto>>();
			_updateValidatorMock = new Mock<IValidator<JobApplicationUpdateDto>>();

			// Default: validators succeed
			_createValidatorMock
				 .Setup(v => v.ValidateAsync(It.IsAny<JobApplicationCreateDto>(), default))
				 .ReturnsAsync(new ValidationResult());

			_updateValidatorMock
				 .Setup(v => v.ValidateAsync(It.IsAny<JobApplicationUpdateDto>(), default))
				 .ReturnsAsync(new ValidationResult());

			_service = new JobApplicationService(
				 _repositoryMock.Object,
				 _createValidatorMock.Object,
				 _updateValidatorMock.Object
			);
		}

		[Fact]
		public async Task CreateJobApplication_ShouldReturnCreatedApplication()
		{
			var userId = Guid.NewGuid();
			var dto = new JobApplicationCreateDto
			{
				ApplicationDate = DateOnly.FromDateTime(DateTime.UtcNow),
				CompanyName = "Test Company",
				JobTitle = "Developer",
				ContractType = ContractType.CLT,
				WorkStyle = WorkStyle.Remote,
				WorkLocationState = "SP"
			};

			_repositoryMock
				 .Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
				 .Returns(Task.CompletedTask);

			// Act
			var result = await _service.CreateAsync(userId, dto);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(dto.CompanyName, result.CompanyName);
			Assert.Equal(dto.JobTitle, result.JobTitle);
			Assert.Equal(dto.ContractType, result.ContractType);
			Assert.Equal(dto.WorkStyle, result.WorkStyle);
			Assert.Equal(dto.WorkLocationState, result.WorkLocationState);
			_repositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
		}

		[Fact]
		public async Task GetAllByUserAsync_ShouldReturnApplicationsForUser()
		{
			var userId = Guid.NewGuid();
			var applications = new List<JobApplication>
				{
					 new(userId, DateOnly.FromDateTime(DateTime.UtcNow), "Company A", "Dev", ContractType.CLT, WorkStyle.Remote, "SP"),
					 new(userId, DateOnly.FromDateTime(DateTime.UtcNow), "Company B", "QA", ContractType.PJ, WorkStyle.Hybrid, "RJ")
				};

			_repositoryMock
				 .Setup(r => r.GetAllByUserAsync(userId))
				 .ReturnsAsync(applications);

			var result = await _service.GetAllByUserAsync(userId);

			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			Assert.Collection(result,
				 app =>
				 {
					 Assert.Equal("Company A", app.CompanyName);
					 Assert.Equal("Dev", app.JobTitle);
					 Assert.Equal(ContractType.CLT, app.ContractType);
					 Assert.Equal(WorkStyle.Remote, app.WorkStyle);
				 },
				 app =>
				 {
					 Assert.Equal("Company B", app.CompanyName);
					 Assert.Equal("QA", app.JobTitle);
					 Assert.Equal(ContractType.PJ, app.ContractType);
					 Assert.Equal(WorkStyle.Hybrid, app.WorkStyle);
				 }
			);

			_repositoryMock.Verify(r => r.GetAllByUserAsync(userId), Times.Once);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnApplication_WhenExists()
		{
			var appId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var application = new JobApplication(
				 userId,
				 DateOnly.FromDateTime(DateTime.UtcNow),
				 "Company X",
				 "Developer",
				 ContractType.CLT,
				 WorkStyle.Remote,
				 "SP"
			);

			_repositoryMock
				 .Setup(r => r.GetByIdAsync(appId))
				 .ReturnsAsync(application);

			var result = await _service.GetByIdAsync(appId);

			Assert.NotNull(result);
			Assert.Equal(application.Id, result!.Id);
			Assert.Equal(application.CompanyName, result.CompanyName);
			Assert.Equal(application.JobTitle, result.JobTitle);
			Assert.Equal(application.ContractType, result.ContractType);
			Assert.Equal(application.WorkStyle, result.WorkStyle);
			Assert.Equal(application.WorkLocationState, result.WorkLocationState);

			_repositoryMock.Verify(r => r.GetByIdAsync(appId), Times.Once);
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
		{
			var appId = Guid.NewGuid();

			_repositoryMock
				 .Setup(r => r.GetByIdAsync(appId))
				 .ReturnsAsync((JobApplication?)null);

			var result = await _service.GetByIdAsync(appId);

			Assert.Null(result);
			_repositoryMock.Verify(r => r.GetByIdAsync(appId), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateApplication_WhenExists()
		{
			var appId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var existingApp = new JobApplication(
				 userId,
				 DateOnly.FromDateTime(DateTime.UtcNow),
				 "Old Company",
				 "Old Title",
				 ContractType.CLT,
				 WorkStyle.Remote,
				 "SP"
			);

			var updateDto = new JobApplicationUpdateDto
			{
				CompanyName = "New Company",
				JobTitle = "New Title",
				SalaryExpectation = 8000m,
				JobOfferUrl = "http://joboffer.com",
				Source = "LinkedIn",
				Notes = "Updated notes"
			};

			_repositoryMock.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync(existingApp);
			_repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<JobApplication>())).Returns(Task.CompletedTask);

			await _service.UpdateAsync(appId, updateDto);

			Assert.Equal(updateDto.CompanyName, existingApp.CompanyName);
			Assert.Equal(updateDto.JobTitle, existingApp.JobTitle);
			Assert.Equal(updateDto.SalaryExpectation, existingApp.SalaryExpectation);
			Assert.Equal(updateDto.JobOfferUrl, existingApp.JobOfferUrl);
			Assert.Equal(updateDto.Source, existingApp.Source);
			Assert.Equal(updateDto.Notes, existingApp.Notes);

			_repositoryMock.Verify(r => r.GetByIdAsync(appId), Times.Once);
			_repositoryMock.Verify(r => r.UpdateAsync(existingApp), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_ShouldThrowException_WhenApplicationNotFound()
		{
			var appId = Guid.NewGuid();
			var updateDto = new JobApplicationUpdateDto { CompanyName = "New Company" };

			_repositoryMock.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync((JobApplication?)null);

			await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(appId, updateDto));

			_repositoryMock.Verify(r => r.GetByIdAsync(appId), Times.Once);
			_repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<JobApplication>()), Times.Never);
		}

		[Fact]
		public async Task DeleteAsync_ShouldDeleteApplication_WhenExists()
		{
			var appId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var existingApp = new JobApplication(
				 userId,
				 DateOnly.FromDateTime(DateTime.UtcNow),
				 "Company",
				 "Title",
				 ContractType.CLT,
				 WorkStyle.Remote,
				 "SP"
			);

			_repositoryMock.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync(existingApp);
			_repositoryMock.Setup(r => r.DeleteAsync(existingApp)).Returns(Task.CompletedTask);

			await _service.DeleteAsync(appId);

			_repositoryMock.Verify(r => r.GetByIdAsync(appId), Times.Once);
			_repositoryMock.Verify(r => r.DeleteAsync(existingApp), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_ShouldThrowException_WhenApplicationNotFound()
		{
			var appId = Guid.NewGuid();
			_repositoryMock.Setup(r => r.GetByIdAsync(appId)).ReturnsAsync((JobApplication?)null);

			await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(appId));

			_repositoryMock.Verify(r => r.GetByIdAsync(appId), Times.Once);
			_repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<JobApplication>()), Times.Never);
		}
	}
}