namespace ReserveMe.Server.Controllers
{
	using Application.Tables.Queries;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.Tables;

	public class TablesController : ApiControllerBase
	{
		#region READ

		[HttpGet("getTablesByVenueId/{venueId}")]
		public async Task<ActionResult<List<TableDto>>> GetTables(int venueId)
		{
			return await Mediator.Send(new GetTablesQuery(venueId));
		}

		//[HttpGet("getAvailableTables")]
		//public async Task<ActionResult<List<TableDto>>> GetAvailableTables(int venueId, DateTime reservationTime, int guestsCount)
		//{
		//	//return await Mediator.Send(new GetAvailableTablesQuery(venueId, reservationTime, guestsCount));
		//}

		#endregion
	}
}
