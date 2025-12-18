namespace ReserveMe.Server.Controllers
{
	using Application.Reservations.Queries;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.Reservations;

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
	}
}
