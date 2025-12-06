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

		private string? TestResult;

		public async Task LoginUser()
		{
			var result = await _authHelper?.LoginAsync(loginUser)!;

			if (!string.IsNullOrEmpty(result))
			{
				navManager?.NavigateTo("/success");
			}
			else
			{
				navManager?.NavigateTo("/401");
			}
		}

		//TEST
		private async Task CallAuthorizedEndpoint()
		{
			TestResult = "Calling authorized endpoint...";

			try
			{
				var result = await _authHelper?.ReservationsAsync(loginUser)!;

				if (!string.IsNullOrEmpty(result))
				{
					TestResult = $"✅ Success: {result}";
				}
				else
				{
					TestResult = $"❌ Failed";
				}
			}
			catch (Exception ex)
			{
				TestResult = $"Error: {ex.Message}";
			}
		}
	}
}
