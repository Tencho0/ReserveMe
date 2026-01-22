namespace ReserveMe.Pages.Venues
{
	using Microsoft.AspNetCore.Components;
	using Microsoft.AspNetCore.Components.Forms;
	using Microsoft.JSInterop;
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
		[Inject] private IJSRuntime jsRuntime { get; set; }
		[Inject] private IConfiguration Configuration { get; set; } = default!;

		public string? UserId { get; set; }

		private VenueSearchDto venue { get; set; }
		private List<ReviewDto> recentReviews { get; set; }

		private ReviewDto reviewDto = new() { Rating = 0 };
		private EditContext reviewEditContext = default!;

		private bool _mapInitialized;
        //private string GoogleMapsApiKey => "AIzaSyBJnwO6GqCkeycV3dEJ3i8waJlBZYCby4Q"; //Test purposes, use code below
        /*=> Configuration["GoogleMaps:ApiKey"] ?? string.Empty;*/
        private string GoogleMapsApiKey => Configuration["GoogleMaps:ApiKey"] ?? string.Empty;//MillaA

        private bool submitting;
		private string? submitError;
		private string? submitSuccess;

		protected override async Task OnInitializedAsync()
		{
			UserId = await this._authHelper.GetUserId();

			if (string.IsNullOrEmpty(UserId)) UserId = null;

			reviewEditContext = new EditContext(reviewDto);
			this.venue = await this._venuesService.GetVenueById(VenueId);
			this.recentReviews = await this._reviewsService.GetReviewsByVenueId(VenueId);
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (!_mapInitialized && venue is not null && venue.Latitude != 0 && venue.Longitude != 0)
			{
				var elementId = $"map-{venue.Id}";
				try
				{
					await jsRuntime.InvokeVoidAsync(
						"venueMap.init",
						GoogleMapsApiKey,
						elementId,
						venue.Latitude,
						venue.Longitude,
						venue.Name
					);
					_mapInitialized = true;
				}
				catch (JSException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}

			await base.OnAfterRenderAsync(firstRender);
		}

		private async Task SubmitReviewAsync()
		{
			if (venue is null) return;

			if (reviewDto.Rating == 0)
			{
				submitError = "Please rate the venue!";
				return;
			}

			submitting = true;
			submitError = null;
			submitSuccess = null;

			try
			{
				reviewDto.VenueId = VenueId;
				reviewDto.UserId = UserId;
				reviewDto.CreatedAt = DateTime.UtcNow;
				await _reviewsService.CreateReviewAsync(reviewDto);

				this.venue = await this._venuesService.GetVenueById(VenueId);
				this.recentReviews = await this._reviewsService.GetReviewsByVenueId(VenueId);

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

		private RenderFragment RenderStars(int rating) => builder =>
		{
			var count = Math.Clamp(rating, 0, 5);
			var seq = 0;
			for (int i = 1; i <= 5; i++)
			{
				var cls = i <= count ? "star-inline active" : "star-inline";
				builder.OpenElement(seq++, "span");
				builder.AddAttribute(seq++, "class", cls);
				builder.AddContent(seq++, "★");
				builder.CloseElement();
			}
		};

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
