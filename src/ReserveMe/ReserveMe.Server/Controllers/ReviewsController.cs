namespace ReserveMe.Server.Controllers
{
	using Application.Reviews.Commands;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Requests.Reviews;

	public class ReviewsController : ApiControllerBase
	{
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
