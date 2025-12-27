namespace Application.Venues.Queries
{
	using Application.Common.Services.Data;
	using Domain.Entities;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Reviews;
	using Shared.Dtos.Users;
	using Shared.Dtos.Venues;

	public record GetVenueDetailsByVenueIdQuery(int VenueId) : IRequest<VenueDetailsDto>;

	public class GetVenueDetailsByVenueIdQueryHandler
		: IRequestHandler<GetVenueDetailsByVenueIdQuery, VenueDetailsDto>
	{
		private readonly IApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public GetVenueDetailsByVenueIdQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<VenueDetailsDto> Handle(
			GetVenueDetailsByVenueIdQuery request,
			CancellationToken cancellationToken)
		{
			var ownersInRole = await _userManager.GetUsersInRoleAsync("Owner");
			var ownerIds = new HashSet<string>(ownersInRole.Select(u => u.Id));

			var waitersInRole = await _userManager.GetUsersInRoleAsync("Waiter");
			var waiterIds = new HashSet<string>(waitersInRole.Select(u => u.Id));

			var venue = await _context.Venues
				.Include(v => v.VenueReviews)
				.ThenInclude(vr => vr.User)
				.Include(v => v.VenueType)
				.Include(v => v.Users)
				.AsNoTracking()
				.FirstOrDefaultAsync(v => v.Id == request.VenueId);

			if (venue == null) return new VenueDetailsDto(); // Throw and handle not found exception

			var owners = venue.Users
				   .Where(u => u.VenueId == request.VenueId && ownerIds.Contains(u.Id))
				   .Select(u => new UserDto
				   {
					   Id = u.Id,
					   VenueId = u.VenueId,
					   FirstName = u.FirstName,
					   LastName = u.LastName,
					   Email = u.Email,
					   PhoneNumber = u.PhoneNumber,
					   ProfilePicture = u.ProfilePicture,
					   IsActive = u.IsActive
				   })
				   .ToList();

			var waiters = venue.Users
				   .Where(u => u.VenueId == request.VenueId && waiterIds.Contains(u.Id))
				   .Select(u => new UserDto
				   {
					   Id = u.Id,
					   VenueId = u.VenueId,
					   FirstName = u.FirstName,
					   LastName = u.LastName,
					   Email = u.Email,
					   PhoneNumber = u.PhoneNumber,
					   ProfilePicture = u.ProfilePicture,
					   IsActive = u.IsActive
				   })
				   .ToList();

			var reviews = venue.VenueReviews;
			var reviewsDtos = new List<ReviewDto>();

			foreach (var review in reviews)
			{
				var revDto = new ReviewDto()
				{
					Id = review.Id,
					ReviewerName = review.UserId == null
						? "Anonymous"
						: ((review.User.FirstName ?? "") + " " + (review.User.LastName ?? "")).Trim(),
					Rating = review.Rating,
					CreatedAt = review.CreatedAt,
					Comment = review.Comment,
				};

				reviewsDtos.Add(revDto);
			}

			var result = new VenueDetailsDto()
			{
				Id = venue.Id,
				VenueTypeId = venue.VenueTypeId,
				Name = venue.Name,
				Description = venue.Description,
				Longitude = venue.Longitude,
				Latitude = venue.Latitude,
				LogoUrl = venue.LogoUrl,
				ImageUrl = venue.ImageUrl,
				IsActive = venue.IsActive,
				CreatedAt = venue.CreatedAt,
				VenueType = venue.VenueType?.Name,
				VenueReviews = venue.VenueReviews == null
					? new List<ReviewDto>()
					: reviewsDtos,
				Owners = owners,
				Waiters = waiters,
			};

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<VenueSearchDto>(venue);
		}
	}
}
