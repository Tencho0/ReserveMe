namespace Shared.Dtos.Venues
{
	using System;
	using System.ComponentModel.DataAnnotations;

	public class VenueCreateDto
	{
		[Required, StringLength(120, MinimumLength = 2)]
		public string Name { get; set; } = "";

		[StringLength(1000)]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Venue type is required.")]
		public int VenueTypeId { get; set; }

		[Required]
		[Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
		public double Latitude { get; set; }

		[Required]
		[Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
		public double Longitude { get; set; }

		public string? LogoUrl { get; set; }

		public string? ImageUrl { get; set; }
	}
}
