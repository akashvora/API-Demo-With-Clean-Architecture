namespace Movies.Api.Common.AuthenticationEnums
{
	public static class Roles
	{
		public const string Admin = "Admin";
		public const string TrustMember = "TrustMember";
	}

	public static class Policies
	{
		public const string AdminOnly = "AdminOnly";
		public const string TrustMemberOnly = "TrustMemberOnly";
		public const string AdminOrTrustMember = "AdminOrTrustMember";
	}

	public static class AuthConstant
	{
	   public const string ApiKeyHeaderName = "x-api-key";
	}

}