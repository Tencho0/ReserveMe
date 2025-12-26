using Common.Enums;
using Microsoft.AspNetCore.Components;
using Shared.Dtos.Reservations;
using Shared.Helpers;
using Shared.Services.Reservations;

namespace ReserveMe.Pages.Reservations
{
	public partial class History : ComponentBase
	{
		[Inject] private IReservationsService _reservationsService { get; set; } = null!;
		[Inject] private IAuthenticationHelper _authHelper { get; set; } = null!;
		[Inject] public NavigationManager navManager { get; set; } = default!;

		public List<ReservationForClientDto> Reservations { get; set; } = new();

		public string? UserId { get; set; }

		protected override async Task OnInitializedAsync()
		{
			UserId = await _authHelper.GetUserId();

			if (string.IsNullOrEmpty(UserId))
				navManager?.NavigateTo("/login", forceLoad: true);

			Reservations = await _reservationsService.GetReservationsByClientId(UserId!);
		}

		private string GetStatusClass(ReservationStatus status) => status switch
		{
			ReservationStatus.Pending => "reservation-status--pending",
			ReservationStatus.Approved => "reservation-status--approved",
			ReservationStatus.InProgress => "reservation-status--inprogress",
			ReservationStatus.Declined => "reservation-status--declined",
			ReservationStatus.Completed => "reservation-status--completed",
			_ => "reservation-status--pending"
		};

		private string GetStatusLabel(ReservationStatus status) => status switch
		{
			ReservationStatus.Pending => "Pending",
			ReservationStatus.Approved => "Approved",
			ReservationStatus.InProgress => "In Progress",
			ReservationStatus.Declined => "Declined",
			ReservationStatus.Completed => "Completed",
			_ => "Pending"
		};

		protected void OpenReservation(int id)
		{
			Console.WriteLine($"Open reservation {id}");
			navManager.NavigateTo($"/reservation/{id}");
		}
	}
}
