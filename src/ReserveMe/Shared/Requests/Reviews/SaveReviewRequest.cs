namespace Shared.Requests.Reviews
{
	public class SaveReviewRequest
	{
		public string? UserId { get; set; }

		public int VenueId { get; set; }

		// 1 to 5
		public int? Rating { get; set; }

		public string? Comment { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
