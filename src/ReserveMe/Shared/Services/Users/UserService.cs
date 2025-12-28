namespace Shared.Services.Users
{
	using Blazored.LocalStorage;
	using Microsoft.AspNetCore.Components;
	using Microsoft.JSInterop;
	using Shared.Dtos.Users;
	using Shared.Providers;
	using Shared.Requests.Users;

	public class UserService : IUserService
	{
		[Inject] private IJSRuntime jsRuntime { get; set; }
		private readonly IApiProvider _provider;
		private readonly ILocalStorageService _localStorage;

		private const string TokenKey = "authToken";

		public UserService(IApiProvider apiProvider, ILocalStorageService localStorage)
		{
			this._provider = apiProvider;
			_localStorage = localStorage;
		}

		public async Task<UserDto> GetByNameAsync(string username)
		{
			try
			{
				var token = await _localStorage.GetItemAsStringAsync(TokenKey);

				if (!string.IsNullOrEmpty(token))
				{
					token = token.Trim();
					if (token.StartsWith("\"") && token.EndsWith("\""))
						token = token.Trim('\"');

					Dictionary<string, object> queryParams = new Dictionary<string, object>()
					{
						{"username", username},
					};

					var result = await _provider.GetAsync<UserDto>(Endpoints.GetUserByName, null, queryParams, token);

					return result;
				}
				else
				{
					// Redirect to login if token is not available
					return new UserDto();
				}


			}
			catch (Exception ex)
			{
				//TODO: Log error
				//_logger.LogError(ex.Message);

				return new UserDto();
			}
		}

		public async Task<UserDto> GetByIdAsync(string id)
		{
			try
			{
				Dictionary<string, object> queryParams = new Dictionary<string, object>()
				{
					{"id", id},
				};

				var result = await _provider.GetAsync<UserDto>(Endpoints.GetUserById, null, queryParams);

				return result;
			}
			catch (Exception ex)
			{
				//TODO: Log error
				//_logger.LogError(ex.Message);

				return new UserDto();
			}
		}

		public async Task ChangeWaiterMenu(string userId, int? menuId)
		{
			try
			{
				var request = new ChangeUserVenueRequest
				{
					MenuId = menuId,
					UserId = userId
				};

				await _provider.PutAsync<ChangeUserVenueRequest, object>(Endpoints.UpdateWaiterVenue, request, null);
			}
			catch (Exception ex)
			{
				//TODO: log error
				//_logger.LogError(ex.Message);
			}
		}
	}
}
