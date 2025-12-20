using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.Dtos
{
    public class TableDto
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public TableStatus Status { get; set; }
        public bool IsActive { get; set; }

        // Current reservation info
        public int? CurrentReservationId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime? ReservationTime { get; set; }
        public int? GuestCount { get; set; }
    }

    public class CreateTableDto
    {
        [Required]
        public int VenueId { get; set; }

        [Required]
        [Range(1, 999)]
        public int TableNumber { get; set; }

        [Required]
        [Range(1, 50)]
        public int Capacity { get; set; }
    }

    public class UpdateTableDto
    {
        [Range(1, 999)]
        public int? TableNumber { get; set; }

        [Range(1, 50)]
        public int? Capacity { get; set; }

        public bool? IsActive { get; set; }
    }

    public class ChangeTableStatusDto
    {
        [Required]
        public TableStatus Status { get; set; }
    }

    public class TableStatisticsDto
    {
        public int TotalTables { get; set; }
        public int ActiveTables { get; set; }
        public int AvailableTables { get; set; }
        public int OccupiedTables { get; set; }
        public int ReservedTables { get; set; }
        public int InactiveTables { get; set; }
    }
}