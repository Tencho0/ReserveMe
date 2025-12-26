namespace Shared.Dtos.Reviews
{
	public class ReviewDto
	{
		public int Id { get; set; }

		public string? UserId { get; set; }

		public int VenueId { get; set; }

		// 1 to 5
		public int? Rating { get; set; }

		public string? Comment { get; set; }

		public DateTime CreatedAt { get; set; }

		public string? ReviewerName { get; set; }
	}
}
