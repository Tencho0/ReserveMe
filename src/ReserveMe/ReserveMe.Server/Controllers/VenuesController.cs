namespace ReserveMe.Server.Controllers
{
	using Application.Venues.Commands;
	using Application.Venues.Queries;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.Venues;
	using Shared.Requests.Venues;
    using System.Security.Claims;

	[Authorize]
	public class VenuesController : ApiControllerBase
	{
		#region READ

		[HttpGet("getAll")]
		public async Task<ActionResult<List<VenueAdminDto>>> GetVenues()
		{
			return await Mediator.Send(new GetVenuesQuery());
		}

		[HttpGet("getVenuesForClient")]
		[AllowAnonymous]
		public async Task<ActionResult<List<VenueSearchDto>>> GetVenuesForClient()
		{
			return await Mediator.Send(new GetVenuesForClientQuery());
		}

		[HttpGet("getVenueById/{venueId}")]
		[AllowAnonymous]
		public async Task<ActionResult<VenueSearchDto>> GetVenuesForClient(int venueId)
		{
			return await Mediator.Send(new GetVenueByIdQuery(venueId));
		}

		[HttpGet("getVenueDetailsByVenueId/{venueId}")]
		public async Task<ActionResult<VenueDetailsDto>> GetVenueDetailsByVenueId(int venueId)
		{
			return await Mediator.Send(new GetVenueDetailsByVenueIdQuery(venueId));
		}

		#endregion

		#region CREATE

		[HttpPost("create")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesDefaultResponseType]
		public async Task<IActionResult> CreateVenue(SaveVenueRequest venue)
		{
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			await Mediator.Send(new CreateVenueCommand(venue, userId));

			return NoContent();
		}

		#endregion

		#region DELETE
		[HttpDelete("delete")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesDefaultResponseType]
		public async Task<IActionResult> DeleteVenue(DeleteVenueRequest request)
		{
			await Mediator.Send(new DeleteVenueCommand(request.VenueId));

			return NoContent();
		}
		#endregion
	}
}
