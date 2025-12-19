namespace ReserveMe.Server.Controllers
{
	using Application.Reservations.Commands;
	using Application.Reservations.Queries;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.Reservations;
	using Shared.Requests.Reservations;

	// only authenticated users can access this now
	[Authorize]
	//[Authorize(Roles = "SuperAdmin")]
	public class ReservationsController : ApiControllerBase
	{

		#region READ

		[HttpGet("getAll/{venueId}")]
		public async Task<ActionResult<List<ReservationDto>>> GetReservations(int venueId)
		{
			return await Mediator.Send(new GetReservationsQuery(venueId));
		}

		#endregion

		#region UPDATE

		[HttpPut("updateStaus")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesDefaultResponseType]
		public async Task<IActionResult> PutMenuItem(UpdateReservationStatusRequest request)
		{
			await Mediator.Send(new UpdateReservationStatusCommand(request));

			return NoContent();
		}

		#endregion
	}
}
