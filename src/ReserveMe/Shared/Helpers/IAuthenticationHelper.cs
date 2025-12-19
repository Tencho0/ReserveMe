namespace Shared.Helpers
{
	using System.Security.Claims;
	using Shared.Dtos;

	public interface IAuthenticationHelper
	{
		Task<string> RegisterAsync(RegisterUserDto userDto);

		Task<string> LoginAsync(LoginUserDto userDto);

		Task LogoutAsync();

		Task<int> GetUserMenuId();

		Task<string> GetUserName(ClaimsPrincipal user);
	}
}
