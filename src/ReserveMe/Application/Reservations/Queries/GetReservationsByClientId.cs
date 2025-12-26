namespace Application.Reservations.Queries
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Reservations;

	public record GetReservationsByClientIdQuery(string ClientId) : IRequest<List<ReservationForClientDto>>;

	public class GetReservationsByClientIdQueryHandler
		: IRequestHandler<GetReservationsByClientIdQuery, List<ReservationForClientDto>>
	{
		private readonly IApplicationDbContext _context;

		public GetReservationsByClientIdQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<ReservationForClientDto>> Handle(
			GetReservationsByClientIdQuery request,
			CancellationToken cancellationToken)
		{
			var result = await _context.Reservations
				.Where(r => r.UserId == request.ClientId)
				.Include(r => r.Venue)
				.ThenInclude(v => v.VenueType)
				.AsNoTracking()
				.Select(v => new ReservationForClientDto
				{
					Id = v.Id,
					VenueId = v.VenueId,
					VenueName = v.Venue.Name,
					VenueLatitude = v.Venue.Latitude,
					VenueLongitude = v.Venue.Longitude,
					VenueLogo = v.Venue.LogoUrl,
					VenueType = v.Venue.VenueType.Name,
					TableNumber = v.TableNumber,
					GuestsCount = v.GuestsCount,
					ReservationTime = v.ReservationTime,
					Status = v.Status
				})
				.ToListAsync(cancellationToken);

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<List<ReservationForClientDto>>(reservations);
		}
	}
}
