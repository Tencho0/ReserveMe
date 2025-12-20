using Shared.Dtos;
using Shared.Enums;

namespace ReserveMe.Server.Interfaces
{
    public interface ITableService
    {
        Task<IEnumerable<TableDto>> GetTablesByVenueAsync(int venueId, bool includeInactive = false);
        Task<TableDto?> GetTableByIdAsync(int tableId);
        Task<TableStatisticsDto> GetTableStatisticsAsync(int venueId);
        Task<TableDto> CreateTableAsync(CreateTableDto dto);
        Task<TableDto?> UpdateTableAsync(int tableId, UpdateTableDto dto);
        Task<TableDto?> ChangeTableStatusAsync(int tableId, TableStatus newStatus);
        Task<TableDto?> ReleaseTableAsync(int tableId);
        Task<TableDto?> SeatReservationAsync(int tableId);
        Task<bool> DeleteTableAsync(int tableId);
        Task<TableDto?> ToggleTableActiveAsync(int tableId);
    }
}