using Microsoft.AspNetCore.Components;
using ReserveMe.Pages.Owner;

namespace ReserveMe.Pages.Waiter;

public partial class WaiterTableManagement : ComponentBase
{
    // Filter
    private TableStatus? _statusFilter;

    // Data
    private List<WaiterTableViewModel> _tables = new();
    private WaiterTableViewModel? _selectedTable;

    // UI State
    private bool _detailsDrawerOpen = false;

    // Filtered tables (only active for waiter view)
    private IEnumerable<WaiterTableViewModel> FilteredTables => _tables
        .Where(t => t.IsActive)
        .Where(t => !_statusFilter.HasValue || t.Status == _statusFilter.Value)
        .OrderBy(t => t.TableNumber);

    protected override async Task OnInitializedAsync()
    {
        await LoadTables();
    }

    private async Task LoadTables()
    {
        await Task.Delay(300);
        _tables = GetMockTables();
    }

    private void SelectTable(WaiterTableViewModel table)
    {
        _selectedTable = table;
        _detailsDrawerOpen = true;
    }

    private void ChangeTableStatus(WaiterTableViewModel table, TableStatus newStatus)
    {
        table.Status = newStatus;

        if (newStatus == TableStatus.Available)
        {
            table.ReservationId = null;
            table.CustomerName = null;
            table.ReservationTime = null;
        }

        StateHasChanged();
    }

    private void SeatReservation(WaiterTableViewModel table)
    {
        table.Status = TableStatus.Occupied;
        StateHasChanged();
    }

    private void CancelReservation(WaiterTableViewModel table)
    {
        table.Status = TableStatus.Available;
        table.ReservationId = null;
        table.CustomerName = null;
        table.ReservationTime = null;
        StateHasChanged();
    }

    private string GetTableCardClass(WaiterTableViewModel table)
    {
        var classes = new List<string>();

        if (_selectedTable?.Id == table.Id)
            classes.Add("table-card-selected");

        if (!table.IsActive)
            classes.Add("table-card-inactive");

        return string.Join(" ", classes);
    }

    private static string GetStatusColor(TableStatus status) => status switch
    {
        TableStatus.Available => "#4CAF50",
        TableStatus.Occupied => "#F44336",
        TableStatus.Reserved => "#FF9800",
        _ => "#9E9E9E"
    };

    private static string GetStatusBadgeClass(TableStatus status) => status switch
    {
        TableStatus.Available => "bg-success",
        TableStatus.Occupied => "bg-danger",
        TableStatus.Reserved => "bg-warning text-dark",
        _ => "bg-secondary"
    };

    private static string GetStatusText(TableStatus status) => status switch
    {
        TableStatus.Available => "Available",
        TableStatus.Occupied => "Occupied",
        TableStatus.Reserved => "Reserved",
        _ => "Unknown"
    };

    // Mock Data
    private static List<WaiterTableViewModel> GetMockTables() => new()
    {
        new() { Id = Guid.NewGuid(), TableNumber = 1, Capacity = 2, Status = TableStatus.Available, IsActive = true },
        new() { Id = Guid.NewGuid(), TableNumber = 2, Capacity = 4, Status = TableStatus.Occupied, IsActive = true },
        new() { Id = Guid.NewGuid(), TableNumber = 3, Capacity = 4, Status = TableStatus.Available, IsActive = true },
        new() { Id = Guid.NewGuid(), TableNumber = 4, Capacity = 6, Status = TableStatus.Reserved, IsActive = true },
        new() { Id = Guid.NewGuid(), TableNumber = 5, Capacity = 2, Status = TableStatus.Available, IsActive = true },
        new() { Id = Guid.NewGuid(), TableNumber = 6, Capacity = 8, Status = TableStatus.Occupied, IsActive = true, ReservationId = Guid.NewGuid(), CustomerName = "Ivan Petrov", ReservationTime = new TimeSpan(19, 0, 0) },
        new() { Id = Guid.NewGuid(), TableNumber = 7, Capacity = 4, Status = TableStatus.Available, IsActive = false },
        new() { Id = Guid.NewGuid(), TableNumber = 8, Capacity = 4, Status = TableStatus.Reserved, IsActive = true, ReservationId = Guid.NewGuid(), CustomerName = "Maria Georgieva", ReservationTime = new TimeSpan(20, 30, 0) },
        new() { Id = Guid.NewGuid(), TableNumber = 9, Capacity = 6, Status = TableStatus.Available, IsActive = true },
        new() { Id = Guid.NewGuid(), TableNumber = 10, Capacity = 10, Status = TableStatus.Occupied, IsActive = true },
    };
}

public class WaiterTableViewModel
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public TableStatus Status { get; set; }
    public bool IsActive { get; set; }
    public Guid? ReservationId { get; set; }
    public string? CustomerName { get; set; }
    public TimeSpan? ReservationTime { get; set; }
}