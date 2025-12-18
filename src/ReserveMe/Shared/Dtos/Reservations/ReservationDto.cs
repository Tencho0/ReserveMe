namespace Shared.Dtos.Reservations
{
	using Common.Enums;

	public class ReservationDto
	{
		public int Id { get; set; }

		public string? UserId { get; set; }

		public int VenueId { get; set; }

		public int TableNumber { get; set; }

		public int GuestsCount { get; set; }

		public string? ContactName { get; set; }

		public string? ContactPhone { get; set; }

		public string? ContactEmail { get; set; }

		//public string? QRCode { get; set; }

		public DateTime? ReservationTime { get; set; }

		public ReservationStatus Status { get; set; }
	}
}
