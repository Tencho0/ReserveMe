namespace ReserveMe.Server.Controllers
{
	using Application.Users.Queries;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Dtos.Users;

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
	}
}
