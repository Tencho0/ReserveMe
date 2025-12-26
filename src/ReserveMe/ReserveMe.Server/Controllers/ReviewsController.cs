namespace ReserveMe.Server.Controllers
{
	using Application.Reviews.Commands;
	using Application.Reviews.Queries;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.Reviews;
	using Shared.Requests.Reviews;

	public class ReviewsController : ApiControllerBase
	{
		#region CREATE

		[HttpGet("getReviewsByVenueId/{venueId}")]
		public async Task<ActionResult<List<ReviewDto>>> GetVenues(int venueId)
		{
			return await Mediator.Send(new GetReviewsQuery(venueId));
		}

		#endregion

		#region CREATE

		[HttpPost("create")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesDefaultResponseType]
		public async Task<IActionResult> CreateReview(SaveReviewRequest review)
		{
			await Mediator.Send(new SaveReviewCommand(review));

			return NoContent();
		}

		#endregion
	}
}
