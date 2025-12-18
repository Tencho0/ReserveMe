namespace Shared.Helpers
{
	using System.Net.Http.Headers;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Blazored.LocalStorage;
	using Microsoft.AspNetCore.Components.Authorization;
	using Shared.Authorization;
	using Shared.Dtos;
	using Shared.Providers;
	using Shared.Requests;
	using Shared.Services.Users;

	public class AuthenticationHelper : IAuthenticationHelper
	{
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly IUserService _userService;
		private readonly IApiProvider _provider;
		private readonly HttpClient _httpClient;
		private readonly ILocalStorageService _localStorage;

		private const string TokenKey = "authToken";
		private const string ExpiresAtKey = "authTokenExpiresAt";

		public AuthenticationHelper(AuthenticationStateProvider authenticationStateProvider, IUserService userService, IApiProvider provider, HttpClient httpClient, ILocalStorageService localStorage)
		{
			_authenticationStateProvider = authenticationStateProvider;
			_userService = userService;
			_provider = provider;
			_httpClient = httpClient;
			_localStorage = localStorage;
		}

		public async Task<string> LoginAsync(LoginUserDto userDto)
		{
			try
			{
				if (userDto != null)
				{
					//TODO: Automapper
					//var request = _mapper.Map<LoginUserRequest>(userDto);
					var request = new LoginUserRequest
					{
						Email = userDto.Email,
						Password = userDto.Password
					};

					var result = await _provider.PostAsync<LoginUserRequest, AuthResponse>(Endpoints.LoginUser, request);

					if (result != null && !string.IsNullOrEmpty(result.Token))
					{
						await SaveTokenAsync(result);

						//TODO: Save token in localstorage 
						_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);

						return result.Token;
					}
				}
			}
			catch (Exception ex)
			{
				await LogoutAsync();
			}

			return null;
		}

		public async Task<string> RegisterAsync(RegisterUserDto userDto)
		{
			try
			{
				if (userDto != null)
				{
					//TODO: Automapper
					//var request = _mapper.Map<RegisterUserRequest>(userDto);
					var request = new RegisterUserRequest
					{
						FirstName = userDto.FirstName,
						LastName = userDto.LastName,
						Password = userDto.Password,
						Email = userDto.Email,
						ConfirmPassword = userDto.ConfirmPassword
					};

					var result = await _provider.PostAsync<RegisterUserRequest, AuthResponse>(Endpoints.RegisterUser, request);

					if (result != null && !string.IsNullOrEmpty(result.Token))
					{
						await SaveTokenAsync(result);
						return result.Token;
					}
				}
			}
			catch (Exception ex)
			{
				//TODO: log error
			}

			return null;
		}

		public async Task LogoutAsync()
		{
			await _localStorage.RemoveItemAsync(TokenKey);
			await _localStorage.RemoveItemAsync(ExpiresAtKey);

			_httpClient.DefaultRequestHeaders.Authorization = null;
		}

		private async Task SaveTokenAsync(AuthResponse auth)
		{
			await _localStorage.SetItemAsync(TokenKey, auth.Token);
			await _localStorage.SetItemAsync(ExpiresAtKey, auth.ExpiresAt);

			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", auth.Token);
		}

		public async Task<int> GetUserMenuId()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			if (user.Identity.IsAuthenticated)
			{
				var username = await GetUserName(user);
				var currUser = await _userService.GetByNameAsync(await GetUserName(user));

				return currUser?.VenueId ?? 0;
			}

			return 0;
		}

		public async Task<string> GetUserName(ClaimsPrincipal user)
		{
			var userClaims = user.Claims.ToList();

			var userName = userClaims.FirstOrDefault(x => x.Type == "email")?.Value;

			if (userName != null)
			{
				return userName;
			}

			return string.Empty;
		}
	}
}