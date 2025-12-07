namespace ReserveMe.Pages.Account
{
	using Microsoft.AspNetCore.Components;
	using Shared.Dtos;
	using Shared.Helpers;

	public partial class Login
	{
		[Inject]
		private IAuthenticationHelper? _authHelper { get; set; }

		[Inject]
		private NavigationManager? navManager { get; set; }

		private LoginUserDto loginUser = new LoginUserDto();

		public async Task LoginUser()
		{
			var result = await _authHelper?.LoginAsync(loginUser)!;

			if (!string.IsNullOrEmpty(result))
			{
				navManager?.NavigateTo("/");
			}
			else
			{
				navManager?.NavigateTo("/401");
			}
		}
	}
}
