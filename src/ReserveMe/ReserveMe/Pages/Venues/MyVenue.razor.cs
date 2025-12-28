namespace ReserveMe.Pages.Venues
{
	using Microsoft.AspNetCore.Components;
	using Shared.Dtos.Reviews;
	using Shared.Dtos.Users;
	using Shared.Dtos.Venues;
	using Shared.Helpers;
	using Shared.Services.Users;
	using Shared.Services.Venues;

	public partial class MyVenue
	{
		[Inject] private IVenuesService _venuesService { get; set; } = null!;
		[Inject] private IAuthenticationHelper _authHelper { get; set; } = null!;
		[Inject] private IUserService _userService { get; set; } = null!;
		[Inject] private NavigationManager? navManager { get; set; } = null!;

		public int VenueId { get; set; }

		private VenueDetailsDto? venue { get; set; }
		private List<ReviewDto> recentReviews { get; set; } = new();

		private List<UserDto> owners { get; set; } = new();
		private List<UserDto> waiters { get; set; } = new();

		// State for the confirm modal
		private bool isDeleteModalVisible = false;
		private string confirmTitle = string.Empty;
		private string confirmMessage = string.Empty;
		private string deleteWaiterId = string.Empty;

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

		private void ShowRemoveWaiterModal(string email, string id)
		{
			confirmTitle = "Remove waiter";
			confirmMessage = $"Are you sure you want to remove waiter '{email}'?";
			deleteWaiterId = id;
			isDeleteModalVisible = true;
		}

		private async Task ConfirmDelete()
		{
			if (deleteWaiterId != string.Empty)
			{
				await RemoveWaiter(deleteWaiterId);
			}

			ResetDeleteData();
			isDeleteModalVisible = false;
		}

		private void CloseModal()
		{
			isDeleteModalVisible = false;
			ResetDeleteData();
		}

		private void ResetDeleteData()
		{
			deleteWaiterId = string.Empty;
			confirmTitle = string.Empty;
			confirmMessage = string.Empty;
		}

		private async Task RemoveWaiter(string waiterId)
		{
			var selected = waiters.FirstOrDefault(w => w.Id == waiterId);
			if (selected is null) return;

			waiters.Remove(selected);

			await _userService.ChangeWaiterMenu(waiterId, null);

			StateHasChanged();
		}
	}
}
