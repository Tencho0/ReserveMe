namespace Shared.Services.Reviews
{
	using Shared.Dtos.Reviews;
	using Shared.Providers;
	using Shared.Requests.Reviews;

	public class ReviewsService : IReviewsService
	{
		private readonly IApiProvider _provider;

		public ReviewsService(IApiProvider apiProvider)
		{
			this._provider = apiProvider;
		}

		#region GET

		public async Task<List<ReviewDto>> GetReviewsByVenueId(int venueId)
		{
			try
			{
				object[] uriParams = new object[]
				{
					venueId
				};

				var result = await _provider.GetAsync<List<ReviewDto>>(Endpoints.GetReviewsByVenueId + "/{0}", uriParams, null, null);

				return result;
			}
			catch (Exception ex)
			{
				return new List<ReviewDto>();
			}
		}

		#endregion

		#region POST

		public async Task CreateReviewAsync(ReviewDto reviewDto)
		{
			try
			{
				//TODO: Automapper
				//var model = _mapper.Map<SaveReviewRequest>(reviewDto);
				var model = new SaveReviewRequest()
				{
					UserId = reviewDto.UserId,
					VenueId = reviewDto.VenueId,
					Rating = reviewDto.Rating,
					Comment = reviewDto.Comment,
					CreatedAt = reviewDto.CreatedAt
				};

				await _provider.PostAsync<SaveReviewRequest, object>(Endpoints.CreateReview, model, null);
			}
			catch (Exception ex)
			{
				//TODO: Log errors
				//this._logger.LogError(ex.Message);
			}
		}

		#endregion
	}
}
