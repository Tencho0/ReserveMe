namespace ReserveMe.Server.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Shared.Authorization;
	using Shared.Requests;

	// only authenticated users can access this now
	[Authorize]
	//[Authorize(Roles = "SuperAdmin")]
	public class ReservationsController : ApiControllerBase
	{
		//TODO: Test purposes only - remove later
		[HttpPost("reserve")]
		[Authorize(Roles = "SuperAdmin")]
		public ActionResult<AuthResponse> Reserve([FromBody] LoginUserRequest request)
		{
			//TEST
			AuthResponse authResponse = new AuthResponse()
			{
				Token = request.Email
			};
			return Ok(authResponse);
		}
	}
}
