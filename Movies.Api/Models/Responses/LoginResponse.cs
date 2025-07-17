namespace Movies.Api.Models.Responses
{
	public class LoginResponse
	{
		public string UserId { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;
		public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

	}
}
