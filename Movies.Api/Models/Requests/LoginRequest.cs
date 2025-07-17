namespace Movies.Api.Models.Requests
{
	public class LoginRequest
	{
		public string UserId { get; set; } = string.Empty;
		public required string Email { get; set; } = string.Empty;
		public required IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

	}
}
