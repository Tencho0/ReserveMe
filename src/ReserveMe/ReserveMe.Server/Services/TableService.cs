using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Domain.Entities;
using Domain.Enums;
using Shared.Enums;
using Shared.Dtos;
using ReserveMe.Server.Interfaces;

namespace ReserveMe.Server.Services
{
    public class TableService : ITableService
    {
        private readonly ApplicationDbContext _context;

        public TableService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TableDto>> GetTablesByVenueAsync(int venueId, bool includeInactive = false)
        {
            var query = _context.Tables.Where(t => t.VenueId == venueId);

            if (!includeInactive)
                query = query.Where(t => t.IsActive);

            var tables = await query.OrderBy(t => t.TableNumber).ToListAsync();

            var result = new List<TableDto>();
            foreach (var table in tables)
            {
                var dto = MapToDto(table);

                if (table.Status == TableStatus.Reserved)
                {
                    var reservation = await _context.Reservations
                        .Where(r => r.VenueId == venueId &&
                                    r.TableNumber == table.TableNumber &&
                                    r.Status == ReservationStatus.Approved &&
                                    r.ReservationTime.HasValue &&
                                    r.ReservationTime.Value.Date == DateTime.Today)
                        .OrderBy(r => r.ReservationTime)
                        .FirstOrDefaultAsync();

                    if (reservation != null)
                    {
                        dto.CurrentReservationId = reservation.Id;
                        dto.CustomerName = reservation.ContactName;
                        dto.CustomerPhone = reservation.ContactPhone;
                        dto.ReservationTime = reservation.ReservationTime;
                        dto.GuestCount = reservation.GuestsCount;
                    }
                }
                result.Add(dto);
            }

            return result;
        }

        public async Task<TableDto?> GetTableByIdAsync(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);
            return table == null ? null : MapToDto(table);
        }

        public async Task<TableStatisticsDto> GetTableStatisticsAsync(int venueId)
        {
            var tables = await _context.Tables.Where(t => t.VenueId == venueId).ToListAsync();

            return new TableStatisticsDto
            {
                TotalTables = tables.Count,
                ActiveTables = tables.Count(t => t.IsActive),
                AvailableTables = tables.Count(t => t.IsActive && t.Status == TableStatus.Available),
                OccupiedTables = tables.Count(t => t.IsActive && t.Status == TableStatus.Occupied),
                ReservedTables = tables.Count(t => t.IsActive && t.Status == TableStatus.Reserved),
                InactiveTables = tables.Count(t => !t.IsActive)
            };
        }

        public async Task<TableDto> CreateTableAsync(CreateTableDto dto)
        {
            var exists = await _context.Tables
                .AnyAsync(t => t.VenueId == dto.VenueId && t.TableNumber == dto.TableNumber);

            if (exists)
                throw new InvalidOperationException($"Table {dto.TableNumber} already exists.");

            var table = new Table
            {
                VenueId = dto.VenueId,
                TableNumber = dto.TableNumber,
                Capacity = dto.Capacity,
                Status = TableStatus.Available,
                IsActive = true
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            return MapToDto(table);
        }

        public async Task<TableDto?> UpdateTableAsync(int tableId, UpdateTableDto dto)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return null;

            if (dto.TableNumber.HasValue)
                table.TableNumber = dto.TableNumber.Value;

            if (dto.Capacity.HasValue)
                table.Capacity = dto.Capacity.Value;

            if (dto.IsActive.HasValue)
                table.IsActive = dto.IsActive.Value;

            await _context.SaveChangesAsync();
            return MapToDto(table);
        }

        public async Task<TableDto?> ChangeTableStatusAsync(int tableId, TableStatus newStatus)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return null;

            table.Status = newStatus;
            await _context.SaveChangesAsync();

            return MapToDto(table);
        }

        public async Task<TableDto?> ReleaseTableAsync(int tableId)
        {
            return await ChangeTableStatusAsync(tableId, TableStatus.Available);
        }

        public async Task<TableDto?> SeatReservationAsync(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return null;

            if (table.Status != TableStatus.Reserved)
                throw new InvalidOperationException("Table is not reserved.");

            table.Status = TableStatus.Occupied;
            await _context.SaveChangesAsync();

            return MapToDto(table);
        }

        public async Task<bool> DeleteTableAsync(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return false;

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TableDto?> ToggleTableActiveAsync(int tableId)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null) return null;

            table.IsActive = !table.IsActive;
            await _context.SaveChangesAsync();

            return MapToDto(table);
        }

        private static TableDto MapToDto(Table table) => new()
        {
            Id = table.Id,
            VenueId = table.VenueId,
            TableNumber = table.TableNumber,
            Capacity = table.Capacity,
            Status = table.Status,
            IsActive = table.IsActive
        };
    }
}