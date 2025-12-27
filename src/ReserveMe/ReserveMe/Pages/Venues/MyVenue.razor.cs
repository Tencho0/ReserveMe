namespace ReserveMe.Pages.Venues
{
	using Common;
	using Microsoft.AspNetCore.Components;
	using Shared.Dtos.Reviews;
	using Shared.Dtos.Users;
	using Shared.Dtos.Venues;
	using Shared.Helpers;
	using Shared.Services.Venues;

	public partial class MyVenue
	{
		[Inject] private IVenuesService _venuesService { get; set; } = null!;
		[Inject] private IAuthenticationHelper _authHelper { get; set; } = null!;
		[Inject] private NavigationManager? navManager { get; set; } = null!;

		public int VenueId { get; set; }

		private VenueDetailsDto? venue { get; set; }
		private List<ReviewDto> recentReviews { get; set; } = new();

		private List<UserDto> owners { get; set; } = new();
		private List<UserDto> waiters { get; set; } = new();

		protected override async Task OnInitializedAsync()
		{
			try
			{
				VenueId = await _authHelper.GetUserMenuId();

				if (VenueId == 0)
				{
					navManager.NavigateTo("/404", forceLoad: true);
					return;
				}

				this.venue = await this._venuesService.GetMyVenue(VenueId);
				this.owners = venue.Owners;
				this.waiters = venue.Waiters;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
