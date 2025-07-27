using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movies.Api.Common.AuthenticationEnums;
using System.Security.Claims;

namespace Movies.Api.Filters
{
	public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
	{
		private readonly string _apiKey;
		public AdminAuthRequirement(string apiKey)
		{
			_apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
		}

		//public void OnAuthorization(AuthorizationFilterContext context)
		//{
		//	throw new NotImplementedException();
		//}

		public Task HandleAsync(AuthorizationHandlerContext context)
		{
			if (context.User.HasClaim(c => c.Type == "Role" && c.Value == "Admin"))
			{
				context.Succeed(this);
				return Task.CompletedTask;
			}

			var httpContext = context.Resource as Microsoft.AspNetCore.Http.HttpContext;
			if (httpContext is null)
			{
				return Task.CompletedTask;
			}


			if (!httpContext.Request.Headers.TryGetValue(AuthConstant.ApiKeyHeaderName, out var extractedApiKey))
			{
				context.Fail();
				return Task.CompletedTask;
			}

			if (_apiKey != extractedApiKey)
			{
				context.Fail();
				return Task.CompletedTask;
			}

			var Identity = (ClaimsIdentity)httpContext.User.Identity!;
			Identity.AddClaim(new Claim("userid", Guid.Parse("8c2fc5e5-c9d2-4171-981c-3585eb16d2c9").ToString()));
			context.Succeed(this);
			return Task.CompletedTask;

		}
	}
}
