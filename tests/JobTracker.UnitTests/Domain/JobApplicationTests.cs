using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;

namespace JobTracker.UnitTests.Domain
{
	public class JobApplicationTests
	{
		[Fact]
		public void Constructor_SetsDefaults_CreatedAndStatusAndStage()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var applicationDate = DateOnly.FromDateTime(DateTime.UtcNow);

			// Act
			var jobApp = new JobApplication(
				 userId,
				 applicationDate,
				 companyName: "ACME Corp",
				 jobTitle: "Backend Developer",
				 contractType: ContractType.CLT,
				 workStyle: WorkStyle.Remote,
				 workLocationState: "SP"
			);

			// Assert - defaults
			Assert.Equal(ApplicationStage.Applied, jobApp.CurrentStage);
			Assert.Equal(ApplicationStatus.InProgress, jobApp.Status);

			// Assert - properties propagated
			Assert.Equal(userId, jobApp.UserId);
			Assert.Equal(applicationDate, jobApp.ApplicationDate);
			Assert.Equal("ACME Corp", jobApp.CompanyName);
			Assert.Equal("Backend Developer", jobApp.JobTitle);
			Assert.Equal("SP", jobApp.WorkLocationState);

			// Assert - timestamps set (CreatedAt <= UpdatedAt)
			Assert.True(jobApp.CreatedAt <= jobApp.UpdatedAt, "CreatedAt should be less or equal to UpdatedAt");
		}

		[Fact]
		public void ChangeStage_UpdatesStageAndNotes()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var jobApp = new JobApplication(
				 userId,
				 DateOnly.FromDateTime(DateTime.UtcNow),
				 "ACME Corp",
				 "Backend Developer",
				 ContractType.CLT,
				 WorkStyle.Remote,
				 "SP"
			);

			var newStage = ApplicationStage.RHInterview;
			var note = "Scheduled first interview";
			var oldUpdatedAt = jobApp.UpdatedAt;

			// Act
			jobApp.ChangeStage(newStage, note);

			// Assert
			Assert.Equal(newStage, jobApp.CurrentStage);
			Assert.Equal(note, jobApp.Notes);
			Assert.True(jobApp.UpdatedAt > oldUpdatedAt, "UpdatedAt should be updated after changing stage");
		}

		[Fact]
		public void ChangeStatus_UpdatesStatusAndNotes()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var jobApp = new JobApplication(
				 userId,
				 DateOnly.FromDateTime(DateTime.UtcNow),
				 "Globex Corp",
				 "Full Stack Developer",
				 ContractType.PJ,
				 WorkStyle.Hybrid,
				 "RJ"
			);

			var newStatus = ApplicationStatus.Closed;
			var note = "Position filled by another candidate";
			var oldUpdatedAt = jobApp.UpdatedAt;

			// Act
			jobApp.ChangeStatus(newStatus, note);

			// Assert
			Assert.Equal(newStatus, jobApp.Status);
			Assert.Equal(note, jobApp.Notes);
			Assert.True(jobApp.UpdatedAt > oldUpdatedAt, "UpdatedAt should be updated after changing status");
		}

		[Fact]
		public void UpdateFromDto_UpdatesFieldsCorrectly()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var jobApp = new JobApplication(
				 userId,
				 DateOnly.FromDateTime(DateTime.UtcNow),
				 "Globex Corp",
				 "Full Stack Developer",
				 ContractType.PJ,
				 WorkStyle.Hybrid,
				 "RJ"
			);

			var oldUpdatedAt = jobApp.UpdatedAt;

			var newCompany = "Initech";
			var newJobTitle = "Backend Engineer";
			decimal? newSalary = 12000;
			var newJobOfferUrl = "https://jobs.com/offer123";
			var newSource = "LinkedIn";
			var newNotes = "Strong candidate";

			// Act
			jobApp.UpdateFromDto(
				 companyName: newCompany,
				 jobTitle: newJobTitle,
				 salary: newSalary,
				 jobOfferUrl: newJobOfferUrl,
				 source: newSource,
				 notes: newNotes
			);

			// Assert
			Assert.Equal(newCompany, jobApp.CompanyName);
			Assert.Equal(newJobTitle, jobApp.JobTitle);
			Assert.Equal(newSalary, jobApp.SalaryExpectation);
			Assert.Equal(newJobOfferUrl, jobApp.JobOfferUrl);
			Assert.Equal(newSource, jobApp.Source);
			Assert.Equal(newNotes, jobApp.Notes);
			Assert.True(jobApp.UpdatedAt > oldUpdatedAt, "UpdatedAt should be updated after updating fields");
		}
    }
}
