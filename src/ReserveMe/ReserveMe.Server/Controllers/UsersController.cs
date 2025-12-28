namespace ReserveMe.Server.Controllers
{
	using Application.Users.Command;
	using Application.Users.Queries;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.Users;
	using Shared.Requests.Users;

	public class UsersController : ApiControllerBase
	{
		#region READ

		[HttpGet("getUserByName")]
		public async Task<ActionResult<UserDto>> GetUserByName(string username)
		{
			var result = await Mediator.Send(new GetUserByNameQuery(username));

			return result;
		}

		[HttpGet("GetUserById")]
		[Authorize]
		public async Task<ActionResult<UserDto>> GetUserById(string id)
		{
			return await Mediator.Send(new GetUserByIdQuery(id));
		}

		#endregion

		#region PUT

		[HttpPut("updateWaiterVenue")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesDefaultResponseType]
		[Authorize]
		public async Task<IActionResult> ChangeWaiterMenu(ChangeUserVenueRequest request)
		{
			await Mediator.Send(new ChangeUserVenueCommand(request.UserId, request.MenuId));

			return NoContent();
		}

		#endregion
	}
}
