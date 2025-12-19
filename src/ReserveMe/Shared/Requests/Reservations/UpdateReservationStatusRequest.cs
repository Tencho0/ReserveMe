namespace Shared.Requests.Reservations
{
	using Common.Enums;

	public class UpdateReservationStatusRequest
	{
		public int ReservationId { get; set; }

		public int NewStatus { get; set; }
	}
}
