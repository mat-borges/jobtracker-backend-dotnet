using FluentValidation;
using JobTracker.Application.DTOs;

namespace JobTracker.Application.Common.Validators
{
    public class JobApplicationCreateValidator : AbstractValidator<JobApplicationCreateDto>
    {
        public JobApplicationCreateValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("CompanyName is required");

            RuleFor(x => x.JobTitle)
                .NotEmpty().WithMessage("JobTitle is required");

            RuleFor(x => x.ApplicationDate)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("ApplicationDate cannot be in the future");

            RuleFor(x => x.ContractType)
                .IsInEnum().WithMessage("ContractType is invalid");

            RuleFor(x => x.WorkStyle)
                .IsInEnum().WithMessage("WorkStyle is invalid");

            RuleFor(x => x.WorkLocationState)
                .NotEmpty().WithMessage("WorkLocationState is required");

            RuleFor(x => x.SalaryExpectation)
                .GreaterThanOrEqualTo(0).When(x => x.SalaryExpectation.HasValue)
                .WithMessage("SalaryExpectation cannot be negative");

            RuleFor(x => x.JobOfferUrl)
                .Must(url => string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("JobOfferUrl is invalid");

            RuleFor(x => x.CurrentStage)
                .IsInEnum().When(x => x.CurrentStage.HasValue)
                .WithMessage("CurrentStage is invalid");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Status is invalid");
        }
    }
}
