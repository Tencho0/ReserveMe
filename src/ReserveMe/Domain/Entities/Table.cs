namespace Domain.Entities
{
    using Domain.Enums;

    public class Table
    {
        public int Id { get; set; }

        public int VenueId { get; set; }

        public int TableNumber { get; set; }

        public int Capacity { get; set; }

        public TableStatus Status { get; set; } = TableStatus.Available;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Venue Venue { get; set; } = null!;
    }
}