using JobTracker.Application.Common.Exceptions;
using System.Text.Json;

namespace JobTracker.Api.Middleware
{
	public class ExceptionHandlerMiddleware(RequestDelegate next)
	{
		private readonly RequestDelegate _next = next;

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (KeyNotFoundException ex)
			{
				context.Response.StatusCode = StatusCodes.Status404NotFound;
				await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
			}
			catch (UnauthorizedAccessException ex)
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
			}
			catch (ValidationException ex)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsync(JsonSerializer.Serialize(ex.ToResponseObject()));
			}
			catch (Exception ex)
			{
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
			}

		}
	}
}
