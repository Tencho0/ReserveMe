using Common.Enums;

namespace Application.Reservations.Commands
{
	using Application.Common.Services.Data;
	using Domain.Entities;
	using MediatR;
	using Shared.Requests.Reservations;
    using System.ComponentModel.DataAnnotations;
    using Shared.Exceptions;
    using Microsoft.EntityFrameworkCore;
    using System.Data;

    public record CreateReservationCommand(SaveReservationRequest Data) : IRequest;

	public class CreateReservationCommandHandler
		: IRequestHandler<CreateReservationCommand>
	{
		private readonly IApplicationDbContext _context;

        private const int ReservationDurationMinutes = 120;

        public CreateReservationCommandHandler(IApplicationDbContext context)
		{
			_context = context;
		}
        /*
		async Task IRequestHandler<CreateReservationCommand>.Handle(CreateReservationCommand request, CancellationToken cancellationToken)
		{
			var entity = new Reservation()
			{
				UserId = request.Data.UserId,
				VenueId = request.Data.VenueId,
				TableNumber = request.Data.TableNumber,
				GuestsCount = request.Data.GuestsCount,
				ContactName = request.Data.ContactName,
				ContactPhone = request.Data.ContactPhone,
				ContactEmail = request.Data.ContactEmail,
				ReservationTime = request.Data.ReservationTime,
				Status = (ReservationStatus)request.Data.Status,
			};

			await _context.Reservations.AddAsync(entity);
			await _context.SaveChangesAsync(cancellationToken);
		}
		*/

        public async Task Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            var data = request.Data;

            if (data.ReservationTime is null)
                throw new ValidationException("ReservationTime is required.");

            if (data.GuestsCount <= 0)
                throw new ValidationException("GuestsCount must be greater than 0.");

            var venueOk = await _context.Venues
                .AnyAsync(v => v.Id == data.VenueId && v.IsActive && !v.IsDeleted, cancellationToken);

            if (!venueOk)
                throw new ValidationException("Venue not found or inactive.");

            
            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

           
            var venueCapacity = await _context.Tables
                .Where(t => t.VenueId == data.VenueId && t.IsActive)
                .SumAsync(t => (int?)t.Capacity, cancellationToken) ?? 0;

            if (venueCapacity <= 0)
                throw new ValidationException("Venue has no active tables/capacity configured.");

            if (data.GuestsCount > venueCapacity)
                throw new ReservationCapacityExceededException(
                    $"GuestsCount exceeds venue capacity ({venueCapacity}).",
                    0);

            var start = data.ReservationTime.Value;

            
            var lowerBound = start.AddMinutes(-ReservationDurationMinutes);
            var upperBound = start.AddMinutes(ReservationDurationMinutes);

            
            var usedSeats = await _context.Reservations
                .Where(r => r.VenueId == data.VenueId)
                .Where(r => r.ReservationTime != null)
                .Where(r =>
                    r.Status == ReservationStatus.Pending ||
                    r.Status == ReservationStatus.Approved ||
                    r.Status == ReservationStatus.InProgress)
                .Where(r => r.ReservationTime!.Value > lowerBound && r.ReservationTime!.Value < upperBound)
                .SumAsync(r => (int?)r.GuestsCount, cancellationToken) ?? 0;

            var available = venueCapacity - usedSeats;

            if (data.GuestsCount > available)
                throw new ReservationCapacityExceededException(
                    $"Not enough capacity for that time slot. Available seats: {Math.Max(0, available)}.",
                    Math.Max(0, available));

            var entity = new Reservation
            {
                UserId = data.UserId,
                VenueId = data.VenueId,
                TableNumber = data.TableNumber,
                GuestsCount = data.GuestsCount,
                ContactName = data.ContactName,
                ContactPhone = data.ContactPhone,
                ContactEmail = data.ContactEmail,
                ReservationTime = data.ReservationTime,
                Status = (ReservationStatus)data.Status,
            };

            await _context.Reservations.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
        }
    }
}
