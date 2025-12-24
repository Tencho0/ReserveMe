using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.Dtos.Reservations;
using Shared.Dtos.Venues;
using Shared.Helpers;
using Shared.Services.Reservations;
using Shared.Services.Venues;

namespace ReserveMe.Pages.Reservations;

public partial class CreateReservation : ComponentBase
{
	[Parameter] public int VenueId { get; set; }

	[Inject] private IVenuesService _venuesService { get; set; } = null!;
	[Inject] private IReservationsService _reservationsService { get; set; } = null!;
	[Inject] private IAuthenticationHelper _authHelper { get; set; } = null!;
	[Inject] private NavigationManager? navManager { get; set; }

	protected bool IsFavorite { get; set; }
	protected string? SuccessMessage { get; set; }
	protected string? ErrorMessage { get; set; }

	public string? UserId { get; set; }

	private DateTime? ReservationDate { get; set; }
	private TimeSpan? ReservationTime { get; set; }

	protected VenueSearchDto currVenue { get; set; } = new();

	protected ReservationDto reservationDto { get; set; } = new();
	private EditContext reservationCreateContext = default!;

	protected override async Task OnInitializedAsync()
	{
		reservationCreateContext = new EditContext(reservationDto);
		currVenue = await _venuesService.GetVenueById(VenueId);
		
		if (reservationDto.ReservationTime is not null)
		{
			ReservationDate = reservationDto.ReservationTime.Value.Date;
			ReservationTime = reservationDto.ReservationTime.Value.TimeOfDay;
		}

		UserId = await _authHelper.GetUserId();

		if (string.IsNullOrEmpty(UserId))
			navManager?.NavigateTo("/login");

		await base.OnInitializedAsync();
	}

	protected async Task ReserveAsync()
	{
		ErrorMessage = null;
		SuccessMessage = null;
		SyncToDto();
		try
		{
			if (!reservationCreateContext.Validate())
			{
				ErrorMessage = "Please fix the validation errors and try again.";
				return;
			}

			var validationError = ValidateReservation();
			if (validationError is not null)
			{
				ErrorMessage = validationError;
				return;
			}

			reservationDto.UserId = UserId;
			reservationDto.VenueId = VenueId;
			//reservationDto.CreatedAt = DateTime.UtcNow;
			reservationDto.Status = Common.Enums.ReservationStatus.Pending;

			await _reservationsService.CreateReservationAsync(reservationDto);

			SuccessMessage =
				$"Reservation created (mock). Guest: {reservationDto.ContactName}, " +
				$"Guests: {reservationDto.GuestsCount}, " +
				$"Date: {reservationDto.ReservationTime:MM/dd/yyyy HH:mm}.";

			//TODO: Redirect to myreservations
			ResetReservationForm();
		}
		catch (Exception ex)
		{
			ErrorMessage = ex.Message;
		}
	}

	protected void ToggleFav() => IsFavorite = !IsFavorite;

	//protected static string AreaLabel(string? area)
	//	=> string.IsNullOrWhiteSpace(area) ? "—" : (area == "smoking" ? "Smoking" : "Non-smoking");

	private string? ValidateReservation()
	{
		if (string.IsNullOrEmpty(UserId))
			return "You should be logged to create reservation";

		if (VenueId <= 0)
			return "Invalid venue.";

		if (!reservationDto.ReservationTime.HasValue)
			return "Please choose a reservation date & time.";

		if (reservationDto.ReservationTime.Value <= DateTime.Now)
			return "Reservation time must be in the future.";

		if (reservationDto.GuestsCount < 1)
			return "Guests count must be at least 1.";

		if (reservationDto.GuestsCount > 20) //TODO: Get the real numbers from db
			return "Guests count cannot be more than 20.";

		if (string.IsNullOrWhiteSpace(reservationDto.ContactName))
			return "Contact name is required.";

		return null;
	}

	private void ResetReservationForm()
	{
		reservationDto = new ReservationDto();
		reservationCreateContext = new EditContext(reservationDto);
		StateHasChanged();
	}

	private void SyncToDto()
	{
		if (ReservationDate is null || ReservationTime is null)
		{
			reservationDto.ReservationTime = null;
			return;
		}

		reservationDto.ReservationTime = ReservationDate.Value.Date + ReservationTime.Value;
	}
}
