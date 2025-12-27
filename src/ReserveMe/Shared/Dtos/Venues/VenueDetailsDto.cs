namespace Shared.Dtos.Venues
{
	using System;
	using Shared.Dtos.Reviews;
	using Shared.Dtos.Users;

	public class VenueDetailsDto
	{
		public int Id { get; set; }

		public int? VenueTypeId { get; set; }

		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		public double Longitude { get; set; }

		public double Latitude { get; set; }

		public bool IsActive { get; set; }

		public string? LogoUrl { get; set; }

		public string? ImageUrl { get; set; }

		public DateTime? CreatedAt { get; set; }

		public string? VenueType { get; set; }
		public List<ReviewDto> VenueReviews { get; set; } = new List<ReviewDto>();
		public List<UserDto> Owners { get; set; } = new List<UserDto>();
		public List<UserDto> Waiters { get; set; } = new List<UserDto>();
	}
}
