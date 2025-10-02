
namespace JobTracker.Application.Common.Exceptions
{
    public class JobValidationException(IEnumerable<string> errors) : Exception(CreateMessage(errors))
    {
		public List<string> Errors { get; } = [.. errors];

		private static string CreateMessage(IEnumerable<string> errors)
        {
            return "Validation failed: " + string.Join("; ", errors);
        }
    }
}
