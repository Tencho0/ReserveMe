namespace ReserveMe.Pages.Venues
{
	using Microsoft.AspNetCore.Components;
	using Microsoft.AspNetCore.Components.Forms;
	using Shared.Dtos.Reviews;
	using Shared.Dtos.Venues;
	using Shared.Helpers;
	using Shared.Services.Reviews;
	using Shared.Services.Venues;

	public partial class VenueDetails
	{
		[Parameter] public int VenueId { get; set; }

		[Inject] private IVenuesService _venuesService { get; set; } = null!;
		[Inject] private IReviewsService _reviewsService { get; set; } = null!;
		[Inject] private IAuthenticationHelper _authHelper { get; set; } = null!;

		public string? UserId { get; set; }

		private VenueSearchDto venue { get; set; }


		private ReviewDto reviewDto = new() { Rating = 0 };
		private EditContext reviewEditContext = default!;

		private bool submitting;
		private string? submitError;
		private string? submitSuccess;

		protected override async Task OnInitializedAsync()
		{
			UserId = await this._authHelper.GetUserId();

			if (string.IsNullOrEmpty(UserId)) UserId = null;

			reviewEditContext = new EditContext(reviewDto);
			this.venue = await this._venuesService.GetVenueById(VenueId);
		}

		private async Task SubmitReviewAsync()
		{
			if (venue is null) return;

			submitting = true;
			submitError = null;
			submitSuccess = null;

			try
			{
				reviewDto.VenueId = VenueId;
				reviewDto.UserId = UserId;
				reviewDto.CreatedAt = DateTime.UtcNow;
				await _reviewsService.CreateReviewAsync(reviewDto);


				reviewDto = new ReviewDto { Rating = 0, Comment = string.Empty };
				ResetReviewForm();
				submitSuccess = "Thanks for your review!";

				StateHasChanged();
			}
			catch (Exception ex)
			{
				submitError = $"Failed to submit review: {ex.Message}";
			}
			finally
			{
				submitting = false;
			}
		}

		private void SetRating(int value)
		{
			var clamped = Math.Clamp(value, 1, 5);
			if (reviewDto.Rating != clamped)
			{
				reviewDto.Rating = clamped;
				reviewEditContext?.NotifyFieldChanged(new FieldIdentifier(reviewDto, nameof(reviewDto.Rating)));
				StateHasChanged();
			}
		}

		private void ResetReviewForm()
		{
			reviewDto = new ReviewDto();
			reviewEditContext = new EditContext(reviewDto);
			StateHasChanged();
		}
	}
}
