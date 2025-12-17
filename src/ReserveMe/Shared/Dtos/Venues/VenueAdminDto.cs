namespace Shared.Dtos.Venues
{
	using Shared.Dtos.VenueTypes;

	public class VenueAdminDto
	{
		public int Id { get; set; }

		public int? VenueTypeId { get; set; }

		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		public double Longitude { get; set; }

		public double Latitude { get; set; }

		public bool IsActive { get; set; } = true;

		public bool IsDeleted { get; set; }

		public string? LogoUrl { get; set; }

		public DateTime? CreatedAt { get; set; }

		public VenueTypeDto? VenueType { get; set; }
	}
}
