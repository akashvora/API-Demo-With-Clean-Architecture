using Movies.Application.Feature.Authentication.Commands;
using Movies.Application.Feature.Authentication.Interfaces;
using Movies.Domain.Models;

namespace Movies.Application.Feature.Authentication.UsesCases
{
	public class LoginUseCase
	{
		//private readonly IAuthenticationService _authService;
		private readonly IJwtTokenGenerator _tokenGenerator;

		public LoginUseCase(
			//IAuthenticationService authService,
			IJwtTokenGenerator tokenGenerator)
		{
			//_authService = authService;
			_tokenGenerator = tokenGenerator;
		}

		public async Task<LoginUserToken> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
		{
			//var user = await _authService.ValidateUserAsync(command.Email, command.UserId, cancellationToken);
			//if (user is null)
			//	throw new UnauthorizedAccessException("Invalid credentials.");

			var token = _tokenGenerator.GenerateToken(command.UserId, command.Email, command.Roles);
			return new LoginUserToken
			{
				UserId = command.UserId,
				Email = command.Email,
				Roles = command.Roles,
				Token = token
			};
		}

	}
}
