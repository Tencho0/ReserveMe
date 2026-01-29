using System;
using Common.Enums;
using Application.Common.Services.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos.Tables;

namespace Application.Tables.Queries
{
    public record GetAvailableTablesQuery(int VenueId, DateTime ReservationTime, int GuestsCount) : IRequest<List<TableDto>>;

    public class GetAvailableTablesQueryHandler : IRequestHandler<GetAvailableTablesQuery, List<TableDto>>
    {
        private readonly IApplicationDbContext _context;
        private const int ReservationDurationMinutes = 120;

        public GetAvailableTablesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TableDto>> Handle(GetAvailableTablesQuery request, CancellationToken cancellationToken)
        {
            var start = request.ReservationTime;
            var lowerBound = start.AddMinutes(-ReservationDurationMinutes);
            var upperBound = start.AddMinutes(ReservationDurationMinutes);

            // Взимаме номерата на заетите маси
            var occupiedTableNumbers = await _context.Reservations
                .Where(r => r.VenueId == request.VenueId)
                .Where(r => r.ReservationTime != null)
                .Where(r =>
                    r.Status == ReservationStatus.Pending ||
                    r.Status == ReservationStatus.Approved ||
                    r.Status == ReservationStatus.InProgress)
                .Where(r => r.ReservationTime!.Value > lowerBound && r.ReservationTime!.Value < upperBound)
                .Select(r => r.TableNumber)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Взимаме всички активни маси за заведението, които имат достатъчен капацитет и не са заети
            var availableTables = await _context.Tables
                .Where(t => t.VenueId == request.VenueId && t.IsActive)
                .Where(t => t.Capacity >= request.GuestsCount)
                .Where(t => !occupiedTableNumbers.Contains(t.TableNumber))
                .Select(t => new TableDto
                {
                    Id = t.Id,
                    VenueId = t.VenueId,
                    TableNumber = t.TableNumber,
                    Capacity = t.Capacity,
                    IsActive = t.IsActive
                })
                .ToListAsync(cancellationToken);

            return availableTables;
        }
    }
}
