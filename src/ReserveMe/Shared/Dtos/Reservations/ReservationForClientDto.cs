namespace Shared.Dtos.Reservations
{
	using System;
	using Common.Enums;

	public class ReservationForClientDto
	{
		public int Id { get; set; }

		public int VenueId { get; set; }
		public string VenueName { get; set; } = string.Empty;
		public string? VenueLogo { get; set; }
		public double VenueLatitude { get; set; }
		public double VenueLongitude { get; set; }
		public string? VenueType { get; set; }

		public int? TableNumber { get; set; }

		public int? GuestsCount { get; set; }

		public DateTime? ReservationTime { get; set; }

		public ReservationStatus Status { get; set; }
	}
}
