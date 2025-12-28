namespace Shared.Services.Users
{
	using Shared.Dtos.Users;

	public interface IUserService
	{
		Task<UserDto> GetByNameAsync(string username);

		Task<UserDto> GetByIdAsync(string id);

		Task ChangeWaiterMenu(string userId, int? menuId);
	}
}
