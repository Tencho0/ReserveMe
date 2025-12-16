namespace ReserveMe.Pages.Venues
{
	using Microsoft.AspNetCore.Components;
	using Microsoft.AspNetCore.Components.Authorization;
	using Microsoft.AspNetCore.Components.Forms;
	using Microsoft.JSInterop;
	using Shared.Dtos.Venues;
	using Shared.Dtos.VenueTypes;
	using Shared.Services.Media;
	using Shared.Services.Venues;
	using Shared.Services.VenueTypes;

	public partial class VenuesPage
	{
		[Inject] private IVenuesService _venuesService { get; set; } = null!;
		[Inject] private IVenueTypesService _venueTypesService { get; set; } = null!;
		[Inject] private IMediaService _mediaService { get; set; } = null!;
		[Inject] private IJSRuntime jsRuntime { get; set; }
		[Inject] private AuthenticationStateProvider authStateProvider { get; set; } = null!;

		private string env = "https://localhost:7000";

		private List<VenueAdminDto> venues = new List<VenueAdminDto>();

		private List<VenueTypeDto> venueTypes = new List<VenueTypeDto>();

		private VenueCreateDto venueDto = new();
		private EditContext venueEditContext = default!;
		private bool isSubmitting;

		protected override async Task OnInitializedAsync()
		{
			venueEditContext = new EditContext(venueDto);
			var authState = await authStateProvider.GetAuthenticationStateAsync();

			this.venues = await this._venuesService.GetVenues();
			this.venueTypes = await this._venueTypesService.GetAllAsync();
		}

		private string? GetVenueOwner(int menuId)
		{
			//TODO: Get the real owner
			return "Ivan Ivanov";
		}

		private async Task CreateVenue()
		{
			if (isSubmitting) return;
			isSubmitting = true;

			try
			{
				await _venuesService.CreateVenueAsync(venueDto);

				venues = await _venuesService.GetVenues();

				await jsRuntime.InvokeVoidAsync("clickModalClose", "edit-item");

				ResetVenueForm();
			}
			finally
			{
				isSubmitting = false;
			}
		}

		private async Task OnLogoSelected(InputFileChangeEventArgs e)
		{
			var file = e.File;
			if (file != null)
			{
				try
				{
					var result = await _mediaService.UploadImage(file);

					if (result != null && !string.IsNullOrEmpty(result.SavePath))
					{
						venueDto.LogoUrl = result.SavePath;
					}
				}
				catch (Exception ex)
				{ }
			}
		}

		private async Task OnImageSelected(InputFileChangeEventArgs e)
		{
			var file = e.File;
			if (file != null)
			{
				try
				{
					var result = await _mediaService.UploadImage(file);

					if (result != null && !string.IsNullOrEmpty(result.SavePath))
					{
						venueDto.ImageUrl = result.SavePath;
					}
				}
				catch (Exception ex)
				{ }
			}
		}

		private void OnCancelVenue()
		{
			ResetVenueForm();
		}

		private void ResetVenueForm()
		{
			venueDto = new VenueCreateDto();
			venueEditContext = new EditContext(venueDto);
			StateHasChanged();
		}
	}
}
