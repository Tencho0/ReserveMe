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

		//string SelectedImage { get; set; } = "../assets/images/emptyImg.jpg";
		string SelectedLogo { get; set; } = "../assets/images/emptyImg.jpg";
		string SelectedVenueImage { get; set; } = "../assets/images/emptyImg.jpg";

		private List<VenueAdminDto> venues = new List<VenueAdminDto>();

		private List<VenueTypeDto> venueTypes = new List<VenueTypeDto>();

		const long MaxAllowedSize = 10 * 1024 * 1024;

		private VenueCreateDto venueDto = new();
		private EditContext venueEditContext = default!;
		private bool isSubmitting;

		private bool isDeleteModalVisible = false;
		private int deleteItemId;
		private string deleteMessage = "";
		private string title = "";

		protected override async Task OnInitializedAsync()
		{
			venueEditContext = new EditContext(venueDto);
			var authState = await authStateProvider.GetAuthenticationStateAsync();

			this.venues = await this._venuesService.GetVenues();
			this.venueTypes = await this._venueTypesService.GetAllAsync();
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

		private async Task OnLogoFileSelected(InputFileChangeEventArgs e)
		{
			var file = e.File;

			try
			{
				if (file is null) return;

				SelectedLogo = await ToDataUrl(file);

				var result = await _mediaService.UploadImage(file);
				if (result != null && !string.IsNullOrEmpty(result.FileUrl))
					venueDto.LogoUrl = result.FileUrl;
			}
			catch (Exception ex)
			{ }
		}

		private async Task OnVenueImageFileSelected(InputFileChangeEventArgs e)
		{
			var file = e.File;

			try
			{
				if (file is null) return;

				SelectedVenueImage = await ToDataUrl(file);

				var result = await _mediaService.UploadImage(file);
				if (result != null && !string.IsNullOrEmpty(result.FileUrl))
					venueDto.ImageUrl = result.FileUrl;
			}
			catch (Exception ex)
			{ }
		}

		private async Task<string> ToDataUrl(IBrowserFile file)
		{
			using var ms = new MemoryStream();
			await file.OpenReadStream(MaxAllowedSize).CopyToAsync(ms);
			return $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
		}

		private void ShowDeleteConfirmation(string title, int venueId, string venueName)
		{
			if (title == "Delete Venue")
			{
				this.deleteMessage = $"Are you sure you want to delete Venue '{venueName}' ?";
			}

			this.title = title;
			this.deleteItemId = venueId;
			isDeleteModalVisible = true;
		}

		public async Task DeleteVenue(int venueId)
		{
			await _venuesService.DeleteVenue(venueId);

			venues = await _venuesService.GetVenues();
		}

		private void OnCancelVenue()
		{
			ResetVenueForm();
			//SelectedImage = "../assets/images/emptyImg.jpg";
			SelectedLogo = "../assets/images/emptyImg.jpg";
			SelectedVenueImage = "../assets/images/emptyImg.jpg";
		}

		private void ResetVenueForm()
		{
			venueDto = new VenueCreateDto();
			venueEditContext = new EditContext(venueDto);
			StateHasChanged();
		}

		private async Task ConfirmDelete()
		{
			if (deleteItemId == 0) return;

			if (title == "Delete Venue")
				await DeleteVenue(deleteItemId);


			isDeleteModalVisible = false;
		}

		private void CloseModal()
		{
			isDeleteModalVisible = false;
		}
	}
}
