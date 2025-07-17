using Movies.Api.Models.Responses;


namespace Movies.Api.Middleware
{

	public class ValidationMappingMiddleware
	{
		private readonly RequestDelegate _next;
		public ValidationMappingMiddleware(RequestDelegate request)
		{
			_next = request;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			//if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
			//{
			//	context.Request.EnableBuffering();
			//	var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
			//	context.Request.Body.Position = 0;
			//	if (string.IsNullOrWhiteSpace(requestBody))
			//	{
			//		context.Response.StatusCode = StatusCodes.Status400BadRequest;
			//		await context.Response.WriteAsync("Request body cannot be empty.");
			//		return;
			//	}
			//}

			try {
				await _next(context);
			}
			catch (FluentValidation.ValidationException ex)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				var ValidationfailureResponse = new ValidationFailureResponse
				{
					Errors = ex.Errors.Select(e => new ValidationResponse{ 
						PropertyName = e.PropertyName,
						Message = e.ErrorMessage
					})
				};
				//await context.Response.WriteAsync($"Validation error: {ex.Message}");
				await context.Response.WriteAsJsonAsync(ValidationfailureResponse);

			}
			//await _next(context);
		}
	}
}
