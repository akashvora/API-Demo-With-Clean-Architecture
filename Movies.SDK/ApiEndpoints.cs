using System.Numerics;

namespace Movies.Api.SDK
{
	public class ApiEndpoints
	{
		private const string ApiBase = "/api";
		public static class Movies
		{
			private const string Base = ApiBase + "/movies";
			//private const string Base = $"{ApiBase}/movies";
			// String interpolation shouldn’t be used for constants, as const values are resolved at compile time, and interpolation happens at runtime. here interpolation is $"{varName}/value"

			public const string Create = Base;
			//public const string Get = $"{Base}/{{id}}";
			public const string Get = $"{Base}/{{idOrSlug}}";
			public const string GetAll = Base;
			public const string Update = $"{Base}/{{id}}";
			public const string Delete = $"{Base}/{{id}}";

			public const string Rate = $"{Base}/{{id}}/ratings";
			public const string DeleteRating = $"{Base}/{{id}}/ratings";
		}
		public static class Ratings
		{
			private const string Base = $"{ApiBase}/ratings";
			public const string GetUserRatings = $"{Base}/me";
		}

		public static class AuthenticationCls 
		{
			private const string Base = ApiBase + "/Auth";
			public const string Authentication = Base;
		}
	}
}
