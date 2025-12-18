namespace Application.Reservations.Queries
{
	using Application.Common.Services.Data;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using Shared.Dtos.Reservations;
	using Shared.Dtos.Venues;
	using Shared.Dtos.VenueTypes;

	public record GetReservationsQuery(int VenueId) : IRequest<List<ReservationDto>>;

	public class GetReservationsQueryHandler
		: IRequestHandler<GetReservationsQuery, List<ReservationDto>>
	{
		private readonly IApplicationDbContext _context;

		public GetReservationsQueryHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<ReservationDto>> Handle(
			GetReservationsQuery request,
			CancellationToken cancellationToken)
		{
			var result = await _context.Reservations
				.Where(v => v.VenueId == request.VenueId)
				.Include(v => v.ApplicationUser)
				.AsNoTracking()
				.Select(v => new ReservationDto
				{
					Id = v.Id,
					UserId = v.UserId,
					VenueId = v.VenueId,
					TableNumber = v.TableNumber,
					GuestsCount = v.GuestsCount,
					ContactName = v.ContactName ?? $"{v.ApplicationUser.FirstName} {v.ApplicationUser.LastName}",
					ContactPhone = v.ContactPhone ?? v.ApplicationUser.PhoneNumber,
					ContactEmail = v.ContactEmail ?? v.ApplicationUser.Email,
					ReservationTime = v.ReservationTime,
					Status = v.Status,
				})
				.ToListAsync(cancellationToken);

			return result;

			//TODO: Automapper
			//var result = _mapper.Map<List<VenueAdminDto>>(venue);
		}
	}
}
