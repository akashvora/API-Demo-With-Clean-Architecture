//using FluentValidation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Common.Exceptions;
using System.Net;
using Movies.Shared.Models;

namespace Movies.Api.Middleware
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;


		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (AppException ex)
			{
				var problem = new ProblemDetails
				{
					Title = "Application Error",
					Detail = ex.Message,
					Status = ex.StatusCode,
					Instance = httpContext.Request.Path
				};

				httpContext.Response.StatusCode = ex.StatusCode;
				httpContext.Response.ContentType = "application/problem+json";

				//await httpContext.Response.WriteAsJsonAsync(new { error = ex.Message });
				await httpContext.Response.WriteAsJsonAsync(problem);

			}
			catch (ValidationException ex)
			{

				var errors = ex.Errors
					.Select(e => new ValidationError
					{
						PropertyName = e.PropertyName,
						Message = e.ErrorMessage
					}).ToList();

				var problem = new 
				{
					Title = "Validation Failed",
					Detail = "One or more validation errors occurred.",
					Status = 400,
					Instance = httpContext.Request.Path,
					Errors = errors
				};

				httpContext.Response.StatusCode = 400;
				httpContext.Response.ContentType = "application/problem+json";
				//var body = JsonSerializer.Serialize(problem);
				//await httpContext.Response.WriteAsync(body);
				await httpContext.Response.WriteAsJsonAsync(problem);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception occurred");

				//httpContext.Response.ContentType = "application/json";
				httpContext.Response.ContentType = "application/problem+json";

				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				var response = new ProblemDetails
				{
					Title = "Internal Server Error",
					Detail = ex.Message,
					Status = httpContext.Response.StatusCode,
					Instance = httpContext.Request.Path
				};

				//var json = JsonSerializer.Serialize(response);
				//await httpContext.Response.WriteAsync(json);
				await httpContext.Response.WriteAsJsonAsync(response);

			}


		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ExceptionHandlingMiddlewareExtensions
	{
		public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
