namespace Application.Users.Queries
{
	using System;
	using Domain.Entities;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using Shared.Dtos.Users;

	public record GetUserByNameQuery(string username) : IRequest<UserDto>;

	public class GetUserByNameQueryHandler : IRequestHandler<GetUserByNameQuery, UserDto>
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public GetUserByNameQueryHandler(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<UserDto> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByNameAsync(request.username);

			if (user == null) return null;

			var result = new UserDto()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				IsActive = user.IsActive,
				PhoneNumber = user.PhoneNumber,
				VenueId = user.VenueId,
			};

			return result;
		}
	}
}
