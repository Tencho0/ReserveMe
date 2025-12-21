namespace Shared.Dtos.Venues
{
	using Shared.Dtos.VenueTypes;

	public class VenueSearchDto
	{
		public int Id { get; set; }

		public int? VenueTypeId { get; set; }

		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		public double Longitude { get; set; }

		public double Latitude { get; set; }

		public string? LogoUrl { get; set; }

		public string? ImageUrl { get; set; }

		public double? Rating { get; set; }

		public int ReviewCount { get; set; }

		public int TotalReservations { get; set; }

		public VenueTypeDto? VenueType { get; set; }
	}
}
