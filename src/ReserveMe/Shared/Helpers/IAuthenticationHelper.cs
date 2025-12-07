namespace Shared.Helpers
{
	using Shared.Dtos;

	public interface IAuthenticationHelper
	{
		Task<string> RegisterAsync(RegisterUserDto userDto);

		Task<string> LoginAsync(LoginUserDto userDto);

		Task LogoutAsync();
	}
}
