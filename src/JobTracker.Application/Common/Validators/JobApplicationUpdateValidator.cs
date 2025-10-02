using FluentValidation;
using JobTracker.Application.DTOs;

namespace JobTracker.Application.Common.Validators
{
    public class JobApplicationUpdateValidator : AbstractValidator<JobApplicationUpdateDto>
    {
        public JobApplicationUpdateValidator()
        {
            When(x => x.CompanyName != null, () =>
                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage("CompanyName cannot be empty"));

            When(x => x.JobTitle != null, () =>
                RuleFor(x => x.JobTitle)
                    .NotEmpty().WithMessage("JobTitle cannot be empty"));

            When(x => x.ApplicationDate.HasValue, () =>
                RuleFor(x => x.ApplicationDate)
                    .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage("ApplicationDate cannot be in the future"));

            When(x => x.ContractType.HasValue, () =>
                RuleFor(x => x.ContractType)
                    .IsInEnum().WithMessage("ContractType is invalid"));

            When(x => x.WorkStyle.HasValue, () =>
                RuleFor(x => x.WorkStyle)
                    .IsInEnum().WithMessage("WorkStyle is invalid"));

            When(x => x.WorkLocationState != null, () =>
                RuleFor(x => x.WorkLocationState)
                    .NotEmpty().WithMessage("WorkLocationState cannot be empty"));

            When(x => x.SalaryExpectation.HasValue, () =>
                RuleFor(x => x.SalaryExpectation)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("SalaryExpectation cannot be negative"));

            When(x => !string.IsNullOrWhiteSpace(x.JobOfferUrl), () =>
                RuleFor(x => x.JobOfferUrl)
                    .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    .WithMessage("JobOfferUrl is invalid"));

            When(x => x.CurrentStage.HasValue, () =>
                RuleFor(x => x.CurrentStage)
                    .IsInEnum().WithMessage("CurrentStage is invalid"));

            When(x => x.Status.HasValue, () =>
                RuleFor(x => x.Status)
                    .IsInEnum().WithMessage("Status is invalid"));
        }
    }
}
