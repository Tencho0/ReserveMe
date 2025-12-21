namespace Application.Venues.Queries
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Venues;
	using Shared.Dtos.VenueTypes;

	public record GetVenuesForClientQuery() : IRequest<List<VenueSearchDto>>;

	public class GetVenuesForClientQueryHandler
		: IRequestHandler<GetVenuesForClientQuery, List<VenueSearchDto>>
	{
		private readonly IApplicationDbContext _context;

		public GetVenuesForClientQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<VenueSearchDto>> Handle(
			GetVenuesForClientQuery request,
			CancellationToken cancellationToken)
		{
			var result = await _context.Venues
				.Include(v => v.VenueType)
				.Include(v => v.VenueReviews)
				.Include(v => v.Reservations)
				.AsNoTracking()
				.Where(v => !v.IsDeleted)
				.Select(v => new VenueSearchDto
				{
					Id = v.Id,
					VenueTypeId = v.VenueTypeId,
					Name = v.Name,
					Description = v.Description,
					Longitude = v.Longitude,
					Latitude = v.Latitude,
					LogoUrl = v.LogoUrl,
					ImageUrl = v.ImageUrl,
					VenueType = v.VenueType == null
						? null
						: new VenueTypeDto
						{
							Id = v.VenueType.Id,
							Name = v.VenueType.Name
						},
					ReviewCount = v.VenueReviews.Count,
					Rating = v.VenueReviews.Average(r => r.Rating),
					TotalReservations = v.Reservations.Count,
				})
				.ToListAsync(cancellationToken);

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<List<VenueSearchDto>>(venue);
		}
	}
}
