using JobTracker.Domain.Enums;

namespace JobTracker.Application.DTOs
{
    public class JobApplicationCreateDto
    {
        public DateOnly ApplicationDate { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public ContractType ContractType { get; set; }
        public WorkStyle WorkStyle { get; set; }
        public string WorkLocationState { get; set; } = string.Empty;
        public string WorkLocationCity { get; set; } = string.Empty;
        public string WorkLocationCountry { get; set; } = string.Empty;
        public string? JobOfferUrl { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public string? Source { get; set; }
        public string? Notes { get; set; }
        public ApplicationStage? CurrentStage { get; set; } = ApplicationStage.Applied;
        public ApplicationStatus? Status { get; set; } = ApplicationStatus.InProgress;
    }

    public class JobApplicationUpdateDto
    {
        public DateOnly? ApplicationDate { get; set; }
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public ContractType? ContractType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public string? WorkLocationState { get; set; }
        public string? WorkLocationCity { get; set; }
        public string? WorkLocationCountry { get; set; }
        public string? JobOfferUrl { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public string? Source { get; set; }
        public string? Notes { get; set; }
        public ApplicationStage? CurrentStage { get; set; }
        public ApplicationStatus? Status { get; set; }
    }

    public class JobApplicationResponseDto
    {
        public Guid Id { get; set; }
        public DateOnly ApplicationDate { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public ContractType ContractType { get; set; }
        public WorkStyle WorkStyle { get; set; }
        public string? WorkLocationCity { get; set; }
        public string WorkLocationState { get; set; } = string.Empty;
        public string? WorkLocationCountry { get; set; }
        public string? JobOfferUrl { get; set; }
        public ApplicationStage CurrentStage { get; set; }
        public ApplicationStatus Status { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public string? Source { get; set; }
        public string? Notes { get; set; }
        public int ProcessDays { get; set; }
    }
}
