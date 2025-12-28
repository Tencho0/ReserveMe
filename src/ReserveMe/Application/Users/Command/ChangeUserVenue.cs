namespace Application.Users.Command
{
	using Domain.Entities;
	using MediatR;
	using Microsoft.AspNetCore.Identity;

	public record ChangeUserVenueCommand(string UserId, int? VenueId) : IRequest;

	public class ChangeUserVenueCommandHandler : IRequestHandler<ChangeUserVenueCommand>
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public ChangeUserVenueCommandHandler(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		async Task IRequestHandler<ChangeUserVenueCommand>.Handle(ChangeUserVenueCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.UserId);

			if (user == null) { return; } //TODO: Throw and handle not found exception

			user.VenueId = request.VenueId;

			await _userManager.UpdateAsync(user);
		}
	}
}
