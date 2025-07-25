using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Models.Requests;
using Movies.Api.Models.Responses;
using Movies.Application.Feature.Authentication.Commands;
using Movies.Application.Feature.Authentication.UsesCases;

namespace Movies.Api.Controllers
{
	[ApiController]
	[ApiVersion(1.0)]
	public class AuthenticationController : ControllerBase
	{
		private const string TokenSecret = "StoreAndLoadThisSecurely";
		private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
		private readonly IMapper _mapper;


		private readonly LoginUseCase _loginUseCase;
		public AuthenticationController(LoginUseCase loginUseCase, IMapper mapper) {
			_loginUseCase = loginUseCase;
			_mapper = mapper;
		}

		[ApiVersion(1.0)]
		[HttpPost(ApiEndpoints.AuthenticationCls.Authentication)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GenerateToken([FromBody] LoginRequest request, CancellationToken cancellationToken)
		{
			var command = new LoginCommand
			{
				Email = request.Email,
				UserId = request.UserId,
				Roles = request.Roles
			};

			var loginUserToken = await _loginUseCase.HandleAsync(command, cancellationToken);
			if (loginUserToken is null) {
				return Unauthorized();
			}

			var response=_mapper.Map<LoginResponse>(loginUserToken);
			return Ok(response);
		}

		//[HttpPost("token")]
		//public IActionResult GenerateToken(
		//[FromBody] LoginRequest request)
		//{
		//	var tokenHandler = new JwtSecurityTokenHandler();
		//	var key = Encoding.UTF8.GetBytes(TokenSecret);

		//	var claims = new List<Claim>
		//{
		//	new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		//	new(JwtRegisteredClaimNames.Sub, request.Email),
		//	new(JwtRegisteredClaimNames.Email, request.Email),
		//	new("userid", request.UserId.ToString())
		//};

		//	foreach (var claimPair in request.CustomClaims)
		//	{
		//		var jsonElement = (JsonElement)claimPair.Value;
		//		var valueType = jsonElement.ValueKind switch
		//		{
		//			JsonValueKind.True => ClaimValueTypes.Boolean,
		//			JsonValueKind.False => ClaimValueTypes.Boolean,
		//			JsonValueKind.Number => ClaimValueTypes.Double,
		//			_ => ClaimValueTypes.String
		//		};

		//		var claim = new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType);
		//		claims.Add(claim);
		//	}

		//	var tokenDescriptor = new SecurityTokenDescriptor
		//	{
		//		Subject = new ClaimsIdentity(claims),
		//		Expires = DateTime.UtcNow.Add(TokenLifetime),
		//		Issuer = "https://id.nickchapsas.com",
		//		Audience = "https://movies.nickchapsas.com",
		//		SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		//	};

		//	var token = tokenHandler.CreateToken(tokenDescriptor);

		//	var jwt = tokenHandler.WriteToken(token);
		//	return Ok(jwt);
		//}
	}
}
