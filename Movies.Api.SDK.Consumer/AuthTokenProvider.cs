using Microsoft.IdentityModel.Tokens;
using Movies.Api.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Api.SDK.Consumer
{
	public class AuthTokenProvider
	{
		private readonly HttpClient _httpClient;
		private string _cachedToken = string.Empty;
		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		public AuthTokenProvider(HttpClient httpClient)
		{
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient)); 
		}

		public async Task<string> GetTokenAsync(string email, Guid userId, string[] roles)
		{
			
				if (!string.IsNullOrWhiteSpace(_cachedToken))
				{
					var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(_cachedToken);
					var expirationTimeText = jwt.Claims.Single(claim => claim.Type == "exp").Value;
					var ExpirationTime = UnixTimeStampToDateTime(int.Parse(expirationTimeText));

					if (ExpirationTime > DateTime.Now)
					{
						return _cachedToken; // Return the cached token if it is still valid
					}
				}
				await _semaphore.WaitAsync(); // lock to prevent multiple requests from generating a token at the same time
				var response = await _httpClient.PostAsJsonAsync("api/v1/authentication", new {
					userId = "d8566de3-b1a6-4a9b-b842-8e3887a82e41",
					email = "user@example.com",
					roles = "[\"Admin\"]"
				});

				var newToken = await response.Content.ReadAsStringAsync();
				_cachedToken = newToken;
				_semaphore.Release(); // release the lock after generating the token
				return newToken;
		}

			private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
			{
				 var datetime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			     datetime = datetime.AddSeconds(unixTimeStamp).ToLocalTime();
				return datetime;
			}
			private static DateTime GetExpirationTimeFromToken(string token)
				{
					var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(token);
					var expirationClaim = jwt.Claims.SingleOrDefault(claim => claim.Type == "exp");
					if (expirationClaim != null && long.TryParse(expirationClaim.Value, out var unixTime))
					{
						return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
					}
					return DateTime.MinValue; // or throw an exception if preferred
				}

				//if((string.IsNullOrEmpty(email) || userId == Guid.Empty || roles == null || roles.Length == 0))
				//{
				//	throw new ArgumentException("Email, UserId, and Roles must be provided to generate a token.");
				//}
				//await _semaphore.WaitAsync();
				//try
				//{
				//	if (string.IsNullOrEmpty(_cachedToken))
				//	{
				//		var request = new
				//		{
				//			Email = email,
				//			UserId = userId,
				//			Roles = roles
				//		};
				//		var response = await _httpClient.PostAsJsonAsync("api/v1/authentication", request);
				//		response.EnsureSuccessStatusCode();
				//		var tokenResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
				//		if (tokenResponse != null)
				//		{
				//			_cachedToken = tokenResponse.Token;
				//		}
				//	}
				//}
				//finally
				//{
				//	_semaphore.Release();
				//}
			//}
			//return _cachedToken;
		
	}
}
