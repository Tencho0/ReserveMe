using Microsoft.AspNetCore.Components;

namespace ReserveMe.Pages.Owner;

public partial class ReservationManagement : ComponentBase
{
    // Filters
    private string _searchTerm = string.Empty;
    private DateTime? _dateFrom;
    private DateTime? _dateTo;
    private TimeSpan? _timeFrom;
    private TimeSpan? _timeTo;
    private ReservationStatus? _statusFilter;
    private int? _tableNumberFilter;
    private bool _isLoading = false;

    // Time string helpers for input binding
    private string _dateFromString
    {
        get => _dateFrom?.ToString("yyyy-MM-dd") ?? "";
        set => _dateFrom = string.IsNullOrEmpty(value) ? null : DateTime.TryParse(value, out var d) ? d : null;
    }

    private string _dateToString
    {
        get => _dateTo?.ToString("yyyy-MM-dd") ?? "";
        set => _dateTo = string.IsNullOrEmpty(value) ? null : DateTime.TryParse(value, out var d) ? d : null;
    }

    private string _timeFromString
    {
        get => _timeFrom?.ToString(@"hh\:mm") ?? "";
        set => _timeFrom = string.IsNullOrEmpty(value) ? null : TimeSpan.TryParse(value, out var t) ? t : null;
    }

    private string _timeToString
    {
        get => _timeTo?.ToString(@"hh\:mm") ?? "";
        set => _timeTo = string.IsNullOrEmpty(value) ? null : TimeSpan.TryParse(value, out var t) ? t : null;
    }

    // Pagination
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages => (int)Math.Ceiling((double)_filteredReservations.Count() / _pageSize);

    // Data
    private List<ReservationViewModel> _reservations = new();
    private ReservationViewModel? _selectedReservation;
    private bool _showDetailsDialog = false;

    // Filtered and paged data
    private IEnumerable<ReservationViewModel> _filteredReservations => FilterReservations();
    private IEnumerable<ReservationViewModel> _pagedReservations => _filteredReservations
        .Skip((_currentPage - 1) * _pageSize)
        .Take(_pageSize);

    private bool HasActiveFilters =>
        !string.IsNullOrEmpty(_searchTerm) ||
        _dateFrom.HasValue ||
        _dateTo.HasValue ||
        _timeFrom.HasValue ||
        _timeTo.HasValue ||
        _statusFilter.HasValue ||
        _tableNumberFilter.HasValue;

    protected override async Task OnInitializedAsync()
    {
        await LoadReservations();
    }

    private async Task LoadReservations()
    {
        _isLoading = true;
        StateHasChanged();

        await Task.Delay(500);
        _reservations = GetMockReservations();

        _isLoading = false;
        StateHasChanged();
    }

    private IEnumerable<ReservationViewModel> FilterReservations()
    {
        var query = _reservations.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(_searchTerm))
        {
            var term = _searchTerm.ToLower();
            query = query.Where(r =>
                r.CustomerName.ToLower().Contains(term) ||
                r.PhoneNumber.Contains(term));
        }

        if (_dateFrom.HasValue)
            query = query.Where(r => r.ReservationDate >= _dateFrom.Value.Date);

        if (_dateTo.HasValue)
            query = query.Where(r => r.ReservationDate <= _dateTo.Value.Date);

        if (_timeFrom.HasValue)
            query = query.Where(r => r.ReservationTime >= _timeFrom.Value);

        if (_timeTo.HasValue)
            query = query.Where(r => r.ReservationTime <= _timeTo.Value);

        if (_statusFilter.HasValue)
            query = query.Where(r => r.Status == _statusFilter.Value);

        if (_tableNumberFilter.HasValue)
            query = query.Where(r => r.TableNumber == _tableNumberFilter.Value);

        return query.OrderBy(r => r.ReservationDate).ThenBy(r => r.ReservationTime);
    }

    private void ShowToday()
    {
        _dateFrom = DateTime.Today;
        _dateTo = DateTime.Today;
        _timeFrom = null;
        _timeTo = null;
        _currentPage = 1;
        StateHasChanged();
    }

    private void ClearFilters()
    {
        _searchTerm = string.Empty;
        _dateFrom = null;
        _dateTo = null;
        _timeFrom = null;
        _timeTo = null;
        _statusFilter = null;
        _tableNumberFilter = null;
        _currentPage = 1;
        StateHasChanged();
    }

    private void ChangeStatus(ReservationViewModel reservation, ReservationStatus newStatus)
    {
        reservation.Status = newStatus;
        StateHasChanged();
    }

    private void ViewDetails(ReservationViewModel reservation)
    {
        _selectedReservation = reservation;
        _showDetailsDialog = true;
    }

    // Pagination
    private void NextPage()
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            StateHasChanged();
        }
    }

    private void PreviousPage()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            StateHasChanged();
        }
    }

    private void ResetPage()
    {
        _currentPage = 1;
        StateHasChanged();
    }

    // Helpers
    private static string GetInitials(string name)
    {
        if (string.IsNullOrEmpty(name)) return "?";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
            : name[0].ToString().ToUpper();
    }

    private static string GetStatusBadgeClass(ReservationStatus status) => status switch
    {
        ReservationStatus.Pending => "bg-warning text-dark",
        ReservationStatus.Approved => "bg-success",
        ReservationStatus.InProgress => "bg-info",
        ReservationStatus.Declined => "bg-danger",
        ReservationStatus.Completed => "bg-secondary",
        _ => "bg-secondary"
    };

    private static string GetStatusText(ReservationStatus status) => status switch
    {
        ReservationStatus.Pending => "Pending confirmation",
        ReservationStatus.Approved => "Approved",
        ReservationStatus.InProgress => "In progress",
        ReservationStatus.Declined => "Declined",
        ReservationStatus.Completed => "Completed",
        _ => "Unknown"
    };

    // Mock Data
    private static List<ReservationViewModel> GetMockReservations() => new()
    {
        new() { Id = Guid.NewGuid(), CustomerName = "Ivan Petrov", PhoneNumber = "+359 888 123 456", ReservationDate = DateTime.Today, ReservationTime = new TimeSpan(19, 0, 0), GuestCount = 4, TableNumber = 5, Status = ReservationStatus.Pending, Notes = "Birthday celebration - please prepare a cake" },
        new() { Id = Guid.NewGuid(), CustomerName = "Maria Georgieva", PhoneNumber = "+359 899 234 567", ReservationDate = DateTime.Today, ReservationTime = new TimeSpan(20, 30, 0), GuestCount = 2, TableNumber = 3, Status = ReservationStatus.Approved, Notes = "" },
        new() { Id = Guid.NewGuid(), CustomerName = "Georgi Dimitrov", PhoneNumber = "+359 877 345 678", ReservationDate = DateTime.Today, ReservationTime = new TimeSpan(18, 0, 0), GuestCount = 6, TableNumber = 8, Status = ReservationStatus.InProgress, Notes = "Nut allergy" },
        new() { Id = Guid.NewGuid(), CustomerName = "Elena Stoyanova", PhoneNumber = "+359 888 456 789", ReservationDate = DateTime.Today, ReservationTime = new TimeSpan(21, 0, 0), GuestCount = 3, TableNumber = 2, Status = ReservationStatus.Completed, Notes = "" },
        new() { Id = Guid.NewGuid(), CustomerName = "Petar Nikolov", PhoneNumber = "+359 899 567 890", ReservationDate = DateTime.Today.AddDays(-1), ReservationTime = new TimeSpan(19, 30, 0), GuestCount = 5, TableNumber = 6, Status = ReservationStatus.Declined, Notes = "Customer did not show up" },
        new() { Id = Guid.NewGuid(), CustomerName = "Anna Ivanova", PhoneNumber = "+359 877 678 901", ReservationDate = DateTime.Today.AddDays(1), ReservationTime = new TimeSpan(12, 30, 0), GuestCount = 2, TableNumber = 1, Status = ReservationStatus.Pending, Notes = "Business lunch" },
        new() { Id = Guid.NewGuid(), CustomerName = "Stefan Todorov", PhoneNumber = "+359 888 789 012", ReservationDate = DateTime.Today, ReservationTime = new TimeSpan(13, 0, 0), GuestCount = 8, TableNumber = 10, Status = ReservationStatus.Approved, Notes = "Corporate event" },
        new() { Id = Guid.NewGuid(), CustomerName = "Viktoria Petrova", PhoneNumber = "+359 899 890 123", ReservationDate = DateTime.Today, ReservationTime = new TimeSpan(20, 0, 0), GuestCount = 4, TableNumber = 4, Status = ReservationStatus.Pending, Notes = "" },
        new() { Id = Guid.NewGuid(), CustomerName = "Nikolay Hristov", PhoneNumber = "+359 888 111 222", ReservationDate = DateTime.Today.AddDays(2), ReservationTime = new TimeSpan(19, 0, 0), GuestCount = 6, TableNumber = 7, Status = ReservationStatus.Approved, Notes = "Anniversary" },
        new() { Id = Guid.NewGuid(), CustomerName = "Desislava Koleva", PhoneNumber = "+359 877 333 444", ReservationDate = DateTime.Today.AddDays(1), ReservationTime = new TimeSpan(20, 0, 0), GuestCount = 4, TableNumber = 9, Status = ReservationStatus.Pending, Notes = "" },
        new() { Id = Guid.NewGuid(), CustomerName = "Krasimir Metodiev", PhoneNumber = "+359 899 555 666", ReservationDate = DateTime.Today, ReservationTime = new TimeSpan(12, 0, 0), GuestCount = 2, TableNumber = 1, Status = ReservationStatus.Completed, Notes = "Business meeting" },
        new() { Id = Guid.NewGuid(), CustomerName = "Silvia Atanasova", PhoneNumber = "+359 888 777 888", ReservationDate = DateTime.Today.AddDays(-2), ReservationTime = new TimeSpan(19, 30, 0), GuestCount = 5, TableNumber = 6, Status = ReservationStatus.Completed, Notes = "" },
    };
}

public class ReservationViewModel
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public int GuestCount { get; set; }
    public int TableNumber { get; set; }
    public ReservationStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public enum ReservationStatus
{
    Pending,
    Approved,
    InProgress,
    Declined,
    Completed
}