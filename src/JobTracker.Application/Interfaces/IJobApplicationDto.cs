using JobTracker.Domain.Enums;

namespace JobTracker.Application.Interfaces
{
    public interface IJobApplicationDto
    {
        DateOnly ApplicationDate { get; }
        string CompanyName { get; }
        string JobTitle { get; }
        ContractType ContractType { get; }
        WorkStyle WorkStyle { get; }
        string WorkLocationState { get; }
        string WorkLocationCity { get; }
        string WorkLocationCountry { get; }
        decimal? SalaryExpectation { get; }
        string? JobOfferUrl { get; }
        ApplicationStage? CurrentStage { get; }
        ApplicationStatus? Status { get; }
        string? Source { get; }
        string? Notes { get; }
    }

    public interface IJobApplicationUpdateDto
    {
        DateOnly? ApplicationDate { get; }
        string? CompanyName { get; }
        string? JobTitle { get; }
        ContractType? ContractType { get; }
        WorkStyle? WorkStyle { get; }
        string? WorkLocationState { get; }
        string? WorkLocationCity { get; }
        string? WorkLocationCountry { get; }
        decimal? SalaryExpectation { get; }
        string? JobOfferUrl { get; }
        ApplicationStage? CurrentStage { get; }
        ApplicationStatus? Status { get; }
        string? Source { get; }
        string? Notes { get; }
    }
}
