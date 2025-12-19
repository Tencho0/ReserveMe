using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Shared.Enums;
using Shared.Dtos;

namespace ReserveMe.Pages.Waiter;

public partial class WaiterTableManagement : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;

    // Filter
    private TableStatus? _statusFilter;

    // Data
    private List<TableDto> _tables = new();
    private TableDto? _selectedTable;

    // UI State
    private bool _detailsDrawerOpen = false;
    private bool _isLoading = false;

    // TODO: Get from logged user's venue
    private int _venueId = 2;

    // Filtered tables (only active for waiter view)
    private IEnumerable<TableDto> FilteredTables => _tables
        .Where(t => t.IsActive)
        .Where(t => !_statusFilter.HasValue || t.Status == _statusFilter.Value)
        .OrderBy(t => t.TableNumber);

    protected override async Task OnInitializedAsync()
    {
        await LoadTables();
    }

    private async Task LoadTables()
    {
        _isLoading = true;
        try
        {
            var result = await Http.GetFromJsonAsync<List<TableDto>>($"api/tables/venue/{_venueId}?includeInactive=true");
            _tables = result ?? new List<TableDto>();

            // Ако няма данни от API, използвай mock
            if (!_tables.Any())
            {
                _tables = GetMockTables();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tables: {ex.Message}");
            // При грешка използвай mock данни
            _tables = GetMockTables();
        }
        _isLoading = false;
    }

    private void SelectTable(TableDto table)
    {
        _selectedTable = table;
        _detailsDrawerOpen = true;
    }

    private async Task ChangeTableStatus(TableDto table, TableStatus newStatus)
    {
        try
        {
            var response = await Http.PatchAsJsonAsync($"api/tables/{table.Id}/status", new { status = (int)newStatus });
            if (response.IsSuccessStatusCode)
            {
                table.Status = newStatus;
                if (newStatus == TableStatus.Available)
                {
                    table.CurrentReservationId = null;
                    table.CustomerName = null;
                    table.ReservationTime = null;
                }
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            // Fallback - промени локално
            table.Status = newStatus;
            StateHasChanged();
        }
    }

    private async Task SeatReservation(TableDto table)
    {
        try
        {
            var response = await Http.PostAsync($"api/tables/{table.Id}/seat", null);
            if (response.IsSuccessStatusCode)
            {
                table.Status = TableStatus.Occupied;
                StateHasChanged();
            }
        }
        catch
        {
            table.Status = TableStatus.Occupied;
            StateHasChanged();
        }
    }

    private async Task CancelReservation(TableDto table)
    {
        try
        {
            var response = await Http.PostAsync($"api/tables/{table.Id}/release", null);
            if (response.IsSuccessStatusCode)
            {
                table.Status = TableStatus.Available;
                table.CurrentReservationId = null;
                table.CustomerName = null;
                table.ReservationTime = null;
                StateHasChanged();
            }
        }
        catch
        {
            table.Status = TableStatus.Available;
            StateHasChanged();
        }
    }

    private string GetTableCardClass(TableDto table)
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

    // Mock Data (fallback)
    private static List<TableDto> GetMockTables() => new()
    {
        new() { Id = 1, VenueId = 2, TableNumber = 1, Capacity = 2, Status = TableStatus.Available, IsActive = true },
        new() { Id = 2, VenueId = 2, TableNumber = 2, Capacity = 4, Status = TableStatus.Occupied, IsActive = true },
        new() { Id = 3, VenueId = 2, TableNumber = 3, Capacity = 4, Status = TableStatus.Available, IsActive = true },
        new() { Id = 4, VenueId = 2, TableNumber = 4, Capacity = 6, Status = TableStatus.Reserved, IsActive = true },
        new() { Id = 5, VenueId = 2, TableNumber = 5, Capacity = 2, Status = TableStatus.Available, IsActive = true },
        new() { Id = 6, VenueId = 2, TableNumber = 6, Capacity = 8, Status = TableStatus.Occupied, IsActive = true, CustomerName = "Ivan Petrov" },
        new() { Id = 7, VenueId = 2, TableNumber = 7, Capacity = 4, Status = TableStatus.Available, IsActive = false },
        new() { Id = 8, VenueId = 2, TableNumber = 8, Capacity = 4, Status = TableStatus.Reserved, IsActive = true, CustomerName = "Maria Georgieva" },
        new() { Id = 9, VenueId = 2, TableNumber = 9, Capacity = 6, Status = TableStatus.Available, IsActive = true },
        new() { Id = 10, VenueId = 2, TableNumber = 10, Capacity = 10, Status = TableStatus.Occupied, IsActive = true },
    };
}