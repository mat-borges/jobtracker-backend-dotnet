namespace JobTracker.Application.Common.Exceptions
{
    public class ValidationException(List<string> errors) : Exception("One or more validation errors occurred.")
    {
		public List<string> Errors { get; } = errors;

		public object ToResponseObject() => new
        {
            message = Message,
            errors = Errors
        };
    }
}