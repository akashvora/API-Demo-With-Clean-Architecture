using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Feature.Authentication.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication
{
	public class JwtTokenGenerator: IJwtTokenGenerator
	{
		private readonly JwtSettings _jwtSettings;
		public JwtTokenGenerator(IOptions<JwtSettings> options)
		{
			_jwtSettings = options.Value;
		}

		public string GenerateToken(string userId, string userEmail, IEnumerable<string> roles)
		{
			var claims = new List<Claim>
			{ 
				new Claim(JwtRegisteredClaimNames.Sub, userId),
				new Claim(JwtRegisteredClaimNames.Email, userEmail),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new("userid", userId.ToString())
			};

			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token= new JwtSecurityToken(
				issuer:_jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
				signingCredentials: creds
				);

			return new JwtSecurityTokenHandler().WriteToken(token);

		}
	}
}
