namespace Application.Venues.Queries
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Venues;
	using Shared.Dtos.VenueTypes;

	public record GetVenuesQuery() : IRequest<List<VenueAdminDto>>;

	public class GetVenuesQueryHandler
		: IRequestHandler<GetVenuesQuery, List<VenueAdminDto>>
	{
		private readonly IApplicationDbContext _context;

		public GetVenuesQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<VenueAdminDto>> Handle(
			GetVenuesQuery request,
			CancellationToken cancellationToken)
		{
			var result = await _context.Venues
				.Include(v => v.VenueType)
				.AsNoTracking()
				.Where(v => !v.IsDeleted)
				.Select(v => new VenueAdminDto
				{
					Id = v.Id,
					VenueTypeId = v.VenueTypeId,
					Name = v.Name,
					Description = v.Description,
					Longitude = v.Longitude,
					Latitude = v.Latitude,
					IsActive = v.IsActive,
					IsDeleted = v.IsDeleted,
					LogoUrl = v.LogoUrl,
					CreatedAt = v.CreatedAt,
					VenueType = v.VenueType == null
						? null
						: new VenueTypeDto
						{
							Id = v.VenueType.Id,
							Name = v.VenueType.Name
						}
				})
				.ToListAsync(cancellationToken);

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<List<VenueAdminDto>>(venue);
		}
	}
}
