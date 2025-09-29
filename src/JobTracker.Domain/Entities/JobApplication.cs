using JobTracker.Domain.Common;
using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities
{
    public class JobApplication : AuditableEntity
    {
        protected JobApplication() { }

        public JobApplication(
            Guid userId,
            DateOnly applicationDate,
            string companyName,
            string jobTitle,
            ContractType contractType,
            WorkStyle workStyle,
            string workLocationState)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ApplicationDate = applicationDate;
            CompanyName = companyName;
            JobTitle = jobTitle;
            ContractType = contractType;
            WorkStyle = workStyle;
            WorkLocationState = workLocationState;
            CurrentStage = ApplicationStage.Applied;
            Status = ApplicationStatus.InProgress;
            SetCreatedNow();
            SetUpdatedNow();
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public DateOnly ApplicationDate { get; private set; }
        public string CompanyName { get; private set; } = string.Empty;
        public string JobTitle { get; private set; } = string.Empty;
        public ContractType ContractType { get; private set; }
        public WorkStyle WorkStyle { get; private set; }
        public string? WorkLocationCity { get; private set; }
        public string WorkLocationState { get; private set; } = string.Empty;
        public string? WorkLocationCountry { get; private set; }
        public string? JobOfferUrl { get; private set; }
        public ApplicationStage CurrentStage { get; private set; }
        public ApplicationStatus Status { get; private set; }
        public decimal? SalaryExpectation { get; private set; }
        public string? Source { get; private set; }
        public string? Notes { get; private set; }

        // business methods
        public void ChangeStage(ApplicationStage newStage, string? note = null)
        {
            CurrentStage = newStage;
            Notes = note ?? Notes;
            SetUpdatedNow();
        }

        public void ChangeStatus(ApplicationStatus newStatus, string? note = null)
        {
            Status = newStatus;
            Notes = note ?? Notes;
            SetUpdatedNow();
        }

        public int ProcessDays => (DateTime.UtcNow.Date - ApplicationDate.ToDateTime(TimeOnly.MinValue)).Days;

        // simple update method (expand as needed)
        public void UpdateFromDto(string? companyName = null, string? jobTitle = null, decimal? salary = null, string? jobOfferUrl = null, string? source = null, string? notes = null)
        {
            if (!string.IsNullOrWhiteSpace(companyName)) CompanyName = companyName;
            if (!string.IsNullOrWhiteSpace(jobTitle)) JobTitle = jobTitle;
            SalaryExpectation = salary ?? SalaryExpectation;
            JobOfferUrl = jobOfferUrl ?? JobOfferUrl;
            Source = source ?? Source;
            Notes = notes ?? Notes;
            SetUpdatedNow();
        }
    }
}
