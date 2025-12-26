namespace Shared.Services.Reviews
{
	using Shared.Dtos.Reviews;

	public interface IReviewsService
	{
		Task CreateReviewAsync(ReviewDto reviewDto);

		Task<List<ReviewDto>> GetReviewsByVenueId(int venueId);
	}
}
