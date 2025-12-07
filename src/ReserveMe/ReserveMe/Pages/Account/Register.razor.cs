namespace ReserveMe.Pages.Account
{
	using Microsoft.AspNetCore.Components;
	using Shared.Dtos;
	using Shared.Helpers;

	public partial class Register
	{
		[Inject]
		private IAuthenticationHelper? _authHelper { get; set; }

		[Inject]
		private NavigationManager? navManager { get; set; }

		private RegisterUserDto registerUser = new RegisterUserDto();

		private async Task RegisterUser()
		{
			var result = await _authHelper!.RegisterAsync(registerUser);

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
