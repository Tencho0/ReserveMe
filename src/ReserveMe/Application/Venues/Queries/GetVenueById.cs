namespace Application.Venues.Queries
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Venues;
	using Shared.Dtos.VenueTypes;

	public record GetVenueByIdQuery(int VenueId) : IRequest<VenueSearchDto>;

	public class GetVenueByIdQueryHandler
		: IRequestHandler<GetVenueByIdQuery, VenueSearchDto>
	{
		private readonly IApplicationDbContext _context;

		public GetVenueByIdQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<VenueSearchDto> Handle(
			GetVenueByIdQuery request,
			CancellationToken cancellationToken)
		{
			var venue = await _context.Venues
				.Include(v=> v.VenueReviews)
				.Include(v=> v.Reservations)
				.Include(v => v.VenueType)
				.AsNoTracking()
				.FirstOrDefaultAsync(v => v.Id == request.VenueId);

			if (venue == null) return new VenueSearchDto(); // Throw and handle not found exception

			var result = new VenueSearchDto()
			{
				Id = venue.Id,
				VenueTypeId = venue.VenueTypeId,
				Name = venue.Name,
				Description = venue.Description,
				Longitude = venue.Longitude,
				Latitude = venue.Latitude,
				LogoUrl = venue.LogoUrl,
				ImageUrl = venue.ImageUrl,
				VenueType = venue.VenueType == null
						? null
						: new VenueTypeDto
						{
							Id = venue.VenueType.Id,
							Name = venue.VenueType.Name
						},
				ReviewCount = venue.VenueReviews.Count,
				Rating = venue.VenueReviews.Average(r => r.Rating),
				TotalReservations = venue.Reservations.Count,
			};

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<VenueSearchDto>(venue);
		}
	}
}
