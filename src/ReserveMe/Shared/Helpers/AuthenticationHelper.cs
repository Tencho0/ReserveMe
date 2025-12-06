namespace Shared.Helpers
{
	using System.Net.Http.Headers;
	using System.Threading.Tasks;
	using Blazored.LocalStorage;
	using Shared.Dtos;
	using Shared.Providers;
	using Shared.Requests;

	public class AuthenticationHelper : IAuthenticationHelper
	{
		private readonly IApiProvider _provider;
		private readonly HttpClient _httpClient;
		private readonly ILocalStorageService _localStorage;

		private const string TokenKey = "authToken";
		private const string ExpiresAtKey = "authTokenExpiresAt";

		public AuthenticationHelper(IApiProvider provider, HttpClient httpClient, ILocalStorageService localStorage)
		{
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
				//await LogoutAsync();
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

		private async Task SaveTokenAsync(AuthResponse auth)
		{
			// 1. Save in local storage
			await _localStorage.SetItemAsync(TokenKey, auth.Token);
			await _localStorage.SetItemAsync(ExpiresAtKey, auth.ExpiresAt);

			// 2. Set Authorization header for current HttpClient
			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", auth.Token);
		}

		public async Task LogoutAsync()
		{
			// Clear local storage
			await _localStorage.RemoveItemAsync(TokenKey);
			await _localStorage.RemoveItemAsync(ExpiresAtKey);

			// Remove Authorization header
			_httpClient.DefaultRequestHeaders.Authorization = null;
		}



		// TEST AUTHORIZED CONTROLLER ATTRIBUTES AND JWT TOKEN
		public async Task<string> ReservationsAsync(LoginUserDto userDto)
		{
			try
			{
				if (userDto != null)
				{
					var request = new LoginUserRequest
					{
						Email = userDto.Email,
						Password = userDto.Password
					};

					var result = await _provider.PostAsync<LoginUserRequest, AuthResponse>(Endpoints.Reservations, request);

					if (result != null && !string.IsNullOrEmpty(result.Token))
					{
						return result.Token;
					}
				}
			}
			catch (Exception ex)
			{
				//await LogoutAsync();
			}

			return null;
		}
	}
}