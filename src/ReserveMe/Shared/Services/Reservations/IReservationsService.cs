namespace Shared.Services.Reservations
{
	using Common.Enums;
	using Shared.Dtos.Reservations;

	public interface IReservationsService
	{
		Task<List<ReservationDto>> GetReservations(int venueId);

		Task<bool> ChangeReservationStatus(int orderId, ReservationStatus newStatus);
	}
}
