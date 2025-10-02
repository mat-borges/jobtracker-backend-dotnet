using JobTracker.Application.Common.Exceptions;
using JobTracker.Application.Interfaces;
using JobTracker.Domain.Enums;

namespace JobTracker.Application.Common.Validations
{
	public static class ValidationHelper
	{
		public static void NotNullOrEmpty(string value, string fieldName, List<string> errors)
		{
			if (string.IsNullOrWhiteSpace(value))
				errors.Add($"{fieldName} is required");
		}

		public static void ValidUrl(string? url, string fieldName, List<string> errors)
		{
			if (!string.IsNullOrWhiteSpace(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
				errors.Add($"{fieldName} is invalid");
		}

		public static void NotFutureDate(DateOnly date, string fieldName, List<string> errors)
		{
			if (date > DateOnly.FromDateTime(DateTime.UtcNow))
				errors.Add($"{fieldName} cannot be in the future");
		}

		public static void ValidateJobApplicationDto(IJobApplicationDto dto)
		{
			var errors = new List<string>();

			NotNullOrEmpty(dto.CompanyName, nameof(dto.CompanyName), errors);
			NotNullOrEmpty(dto.JobTitle, nameof(dto.JobTitle), errors);
			NotFutureDate(dto.ApplicationDate, nameof(dto.ApplicationDate), errors);

			if (!Enum.IsDefined(dto.ContractType))
				errors.Add($"{nameof(dto.ContractType)} is invalid");

			if (!Enum.IsDefined(dto.WorkStyle))
				errors.Add($"{nameof(dto.WorkStyle)} is invalid");

			NotNullOrEmpty(dto.WorkLocationState, nameof(dto.WorkLocationState), errors);

			if (dto.SalaryExpectation != null && dto.SalaryExpectation < 0)
				errors.Add($"{nameof(dto.SalaryExpectation)} cannot be negative");

			ValidUrl(dto.JobOfferUrl, nameof(dto.JobOfferUrl), errors);

			if (dto.CurrentStage != null && !Enum.IsDefined(typeof(ApplicationStage), dto.CurrentStage))
				errors.Add($"{nameof(dto.CurrentStage)} is invalid");

			if (dto.Status != null && !Enum.IsDefined(typeof(ApplicationStatus), dto.Status))
				errors.Add($"{nameof(dto.Status)} is invalid");

			if (errors.Count != 0)
				throw new ValidationException(errors);
		}

		public static void ValidateJobApplicationUpdateDto(IJobApplicationUpdateDto dto)
		{
			var errors = new List<string>();

			if (dto.CompanyName != null)
				NotNullOrEmpty(dto.CompanyName, nameof(dto.CompanyName), errors);

			if (dto.JobTitle != null)
				NotNullOrEmpty(dto.JobTitle, nameof(dto.JobTitle), errors);

			if (dto.ApplicationDate.HasValue)
				NotFutureDate(dto.ApplicationDate.Value, nameof(dto.ApplicationDate), errors);

			if (dto.ContractType.HasValue && !Enum.IsDefined(dto.ContractType.Value))
				errors.Add($"{nameof(dto.ContractType)} is invalid");

			if (dto.WorkStyle.HasValue && !Enum.IsDefined(dto.WorkStyle.Value))
				errors.Add($"{nameof(dto.WorkStyle)} is invalid");

			if (dto.WorkLocationState != null)
				NotNullOrEmpty(dto.WorkLocationState, nameof(dto.WorkLocationState), errors);

			if (dto.SalaryExpectation.HasValue && dto.SalaryExpectation < 0)
				errors.Add($"{nameof(dto.SalaryExpectation)} cannot be negative");

			if (!string.IsNullOrWhiteSpace(dto.JobOfferUrl))
				ValidUrl(dto.JobOfferUrl, nameof(dto.JobOfferUrl), errors);

			if (dto.CurrentStage.HasValue && !Enum.IsDefined(dto.CurrentStage.Value))
				errors.Add($"{nameof(dto.CurrentStage)} is invalid");

			if (dto.Status.HasValue && !Enum.IsDefined(dto.Status.Value))
				errors.Add($"{nameof(dto.Status)} is invalid");

			if (errors.Count != 0)
				throw new ValidationException(errors);
		}
	}
}
