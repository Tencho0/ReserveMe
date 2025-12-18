namespace ReserveMe.Server.Controllers
{
	using Application.Users.Queries;
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

		#endregion
	}
}
