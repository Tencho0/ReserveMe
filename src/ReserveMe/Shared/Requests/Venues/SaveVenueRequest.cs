namespace Shared.Requests.Venues
{
	public class SaveVenueRequest
	{
		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		public int? VenueTypeId { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public string? LogoUrl { get; set; }

		public string? ImageUrl { get; set; }
	}
}
