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
        public string? WorkLocationCity { get; set; }
        public string? WorkLocationCountry { get; set; }
        public string? JobOfferUrl { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public string? Source { get; set; }
        public string? Notes { get; set; }
    }

    public class JobApplicationUpdateDto
    {
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public string? JobOfferUrl { get; set; }
        public string? Source { get; set; }
        public string? Notes { get; set; }
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
