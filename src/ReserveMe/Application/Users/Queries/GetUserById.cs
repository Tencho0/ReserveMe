namespace Application.Users.Queries
{
	using Domain.Entities;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using Shared.Dtos.Users;

	public record GetUserByIdQuery(string Id) : IRequest<UserDto>;

	public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.Id);

			if (user == null) return null;

			var result = new UserDto
			{
				Id = user.Id,
				VenueId = user.VenueId,
				FirstName = user.FirstName,
				LastName = user.LastName,
				ProfilePicture = user.ProfilePicture,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				IsActive = user.IsActive,
			};

			return result;
		}
	}

}
