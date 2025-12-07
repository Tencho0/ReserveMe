namespace Shared.Authorization
{
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Blazored.LocalStorage;
	using Microsoft.AspNetCore.Components.Authorization;

	public class ApiAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorage;
		private readonly JwtSecurityTokenHandler _tokenHandler = new();

		private const string TokenKey = "authToken";

		public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
		{
			this._localStorage = localStorage;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _localStorage.GetItemAsStringAsync(TokenKey);

			if (string.IsNullOrWhiteSpace(token))
			{
				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
			}

			token = token.Trim();

			if (token.StartsWith("\"") && token.EndsWith("\""))
				token = token.Trim('"');

			var jwtToken = _tokenHandler.ReadJwtToken(token);

			var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
			var user = new ClaimsPrincipal(identity);

			return new AuthenticationState(user);
		}

		public void MarkUserAsAuthenticated(string token)
		{
			var jwtToken = _tokenHandler.ReadJwtToken(token);
			var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
			var user = new ClaimsPrincipal(identity);

			NotifyAuthenticationStateChanged(
				Task.FromResult(new AuthenticationState(user)));
		}

		public void MarkUserAsLoggedOut()
		{
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(
				Task.FromResult(new AuthenticationState(anonymous)));
		}
	}
}
