namespace Shared.Services.Reviews
{
	using Shared.Dtos.Reviews;

	public interface IReviewsService
	{
		Task CreateReviewAsync(ReviewDto reviewDto);
	}
}
