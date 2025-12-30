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

		#endregion
	}
}
