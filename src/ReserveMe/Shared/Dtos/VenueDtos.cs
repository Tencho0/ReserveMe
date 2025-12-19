namespace Shared.Dtos
{
	// Response DTO for venue search
	public class VenueSearchDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? VenueTypeName { get; set; }
		public int? VenueTypeId { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public string? LogoUrl { get; set; }
		public string? ImageUrl { get; set; }
		public double Rating { get; set; }
		public int ReviewCount { get; set; }
		public int TotalReservations { get; set; }
		public double Distance { get; set; }
		public bool IsFavorite { get; set; }
		public string? Address { get; set; }
	}

	// Search request DTO
	public class VenueSearchRequestDto
	{
		public double? UserLatitude { get; set; }
		public double? UserLongitude { get; set; }
		public int RadiusKm { get; set; } = 5;
		public string? SearchTerm { get; set; }
        public List<int>? VenueTypeIds { get; set; }
        public string SortBy { get; set; } = "rating"; // rating, distance, reservations
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 6;
	}

	// Paginated response
	public class VenueSearchResultDto
	{
		public List<VenueSearchDto> Venues { get; set; } = new();
		public int TotalCount { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public bool HasMore { get; set; }
	}

	// Venue detail DTO
	public class VenueDetailDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? VenueTypeName { get; set; }
		public int? VenueTypeId { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public string? LogoUrl { get; set; }
		public string? ImageUrl { get; set; }
		public double Rating { get; set; }
		public int ReviewCount { get; set; }
		public int TotalReservations { get; set; }
		public int TotalTables { get; set; }
		public int AvailableTables { get; set; }
		public bool IsFavorite { get; set; }
		public List<VenueReviewDto> RecentReviews { get; set; } = new();
	}

	// Review DTO
	public class VenueReviewDto
	{
		public int Id { get; set; }
		public string? UserName { get; set; }
		public int? Rating { get; set; }
		public string? Comment { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	// VenueType DTO
	public class VenueTypeDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
	}

	// Create Venue DTO
	public class CreateVenueDto
	{
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int? VenueTypeId { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public string? LogoUrl { get; set; }
		public string? ImageUrl { get; set; }
	}

	// Toggle favorite DTO
	public class ToggleFavoriteDto
	{
		public int VenueId { get; set; }
	}
}