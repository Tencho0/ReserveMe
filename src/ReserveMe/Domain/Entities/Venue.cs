namespace Domain.Entities
{
	public class Venue
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

		public string? ImageUrl { get; set; }
		 
		public DateTime? CreatedAt { get; set; }

		// Navigation properties
		public VenueType? VenueType { get; set; }
		public ICollection<VenueFavorite> Favorites { get; set; } = new HashSet<VenueFavorite>();
		public ICollection<VenueReview> VenueReviews { get; set; } = new HashSet<VenueReview>();
		public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
		public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
		public ICollection<Table> Tables { get; set; } = new HashSet<Table>();
	}
}