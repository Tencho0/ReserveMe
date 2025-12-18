namespace Shared.Services.Reservations
{
	using Shared.Dtos.Reservations;

	public interface IReservationsService
	{
		Task<List<ReservationDto>> GetReservations(int venueId);
	}
}
