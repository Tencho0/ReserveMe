namespace ReserveMe.Server.Controllers
{
	using Application.VenueTypes.Queries;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.VenueTypes;

	public class VenueTypesController : ApiControllerBase
	{
		#region READ

		[HttpGet("getAll")]
		public async Task<ActionResult<List<VenueTypeDto>>> GetVenueTypes()
		{
			return await Mediator.Send(new GetVenueTypesQuery());
		}

		#endregion
	}
}
