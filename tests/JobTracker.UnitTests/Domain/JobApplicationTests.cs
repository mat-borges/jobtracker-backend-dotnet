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
    }
}
