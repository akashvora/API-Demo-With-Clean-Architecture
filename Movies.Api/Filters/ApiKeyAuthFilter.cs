using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movies.Api.Common.AuthenticationEnums;

namespace Movies.Api.Filters
{
	public class ApiKeyAuthFilter : IAuthorizationFilter //IAsyncAuthorizationFilter
	{
		private readonly IConfiguration _configuration;
		public ApiKeyAuthFilter(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstant.ApiKeyHeaderName, out var extractedApiKey))
			{ 
				context.Result = new UnauthorizedObjectResult("API Key missing");
				return;
			}

			var apiKey = _configuration["ApiKey"]; //_configuration.GetValue<string>(AuthConstant.ApiKeyHeaderName);

			if (apiKey != extractedApiKey)
			{
				context.Result = new UnauthorizedObjectResult("Invalid API Key");
				//return;
			}
		}

		// below is the aysnc filters implementation
		//public Task OnAuthorizationAsync(AuthorizationFilterContext context)
		//{
		//	throw new NotImplementedException();
		//}
	}
}
