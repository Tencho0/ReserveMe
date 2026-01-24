namespace ReserveMe.Pages.Tables
{
    using Microsoft.AspNetCore.Components;
    using Shared.Dtos.Tables;
    using Shared.Dtos.Reservations;
    using Shared.Helpers;
    using Shared.Services.Tables;
    using Common.Enums;
    using Shared.Services.Reservations;

    public partial class LivePage
    {
        [Inject] private ITablesService _tablesService { get; set; } = null!;
        [Inject] private IReservationsService _reservationsService { get; set; } = null!;
        [Inject] private IAuthenticationHelper _authHelper { get; set; } = null!;
        [Inject] private NavigationManager navManager { get; set; } = null!;

        public int VenueId { get; set; }

        // ================= Filters =================
        private TableStatus? _statusFilter;
        private bool? _activeFilter;

        // ================= Data =================
        private List<TableDto> _tablesFromDb = new();
        private List<ReservationDto> _reservationsFromDb = new();
        private List<TableViewModel> _tables = new();
        private TableViewModel? _selectedTable;
        private TableViewModel _editingTable = new();

        // ================= UI State =================
        private bool _detailsDrawerOpen = false;
        private bool _tableDialogOpen = false;
        private bool _reserveDialogOpen = false;
        private bool _deleteDialogOpen = false;
        private bool _isEditMode = false;

        // ================= Reservation Fields =================
        private string _reservationCustomerName = string.Empty;
        private string _reservationPhoneNumber = string.Empty;
        private DateTime? _reservationDate = DateTime.Today;
        private TimeSpan? _reservationTime = new TimeSpan(19, 0, 0);
        private int _reservationGuestCount = 2;

        private string _reservationTimeString = "19:00";

        // ================= Lifecycle =================
        protected override async Task OnInitializedAsync()
        {
            VenueId = await _authHelper.GetUserMenuId();

            if (VenueId == 0)
            {
                navManager.NavigateTo("/404", forceLoad: true);
                return;
            }

            _tablesFromDb = await _tablesService.GetTablesByVenueId(VenueId);
            _reservationsFromDb = await _reservationsService.GetReservations(VenueId);

            await LoadTables();
        }

        // ================= Data Mapping =================
        private async Task LoadTables()
        {
            await Task.Delay(200);

            _tables = _tablesFromDb
                .Select(table =>
                {
                    var reservation = _reservationsFromDb.FirstOrDefault(r =>
                        r.TableNumber == table.TableNumber &&
                        (r.Status == ReservationStatus.Approved ||
                         r.Status == ReservationStatus.InProgress));

                    var status = reservation switch
                    {
                        null => TableStatus.Available,
                        _ when reservation.Status == ReservationStatus.InProgress => TableStatus.Occupied,
                        _ => TableStatus.Reserved
                    };

                    return new TableViewModel
                    {
                        Id = Guid.NewGuid(),
                        TableNumber = table.TableNumber,
                        Capacity = table.Capacity,
                        IsActive = table.IsActive,
                        Status = status,

                        ActiveReservationId = reservation != null ? Guid.NewGuid() : null,
                        CustomerName = reservation?.ContactName,
                        CustomerPhone = reservation?.ContactPhone,
                        ReservationDate = reservation?.ReservationTime?.Date,
                        ReservationTime = reservation?.ReservationTime?.TimeOfDay,
                        GuestCount = reservation?.GuestsCount
                    };
                })
                .OrderBy(t => t.TableNumber)
                .ToList();
        }

        // ================= Selection =================
        private void SelectTable(TableViewModel table)
        {
            _selectedTable = table;
            _detailsDrawerOpen = true;
        }

        // ================= Filters =================
        private IEnumerable<TableViewModel> FilteredTables => _tables
            .Where(t => !_statusFilter.HasValue || t.Status == _statusFilter.Value)
            .Where(t => !_activeFilter.HasValue || t.IsActive == _activeFilter.Value)
            .OrderBy(t => t.TableNumber);

        // ================= Dialogs =================
        private void OpenAddTableDialog()
        {
            _isEditMode = false;
            _editingTable = new TableViewModel
            {
                Id = Guid.NewGuid(),
                TableNumber = _tables.Any() ? _tables.Max(t => t.TableNumber) + 1 : 1,
                Capacity = 4,
                Status = TableStatus.Available,
                IsActive = true
            };
            _tableDialogOpen = true;
        }

        private void OpenEditTableDialog()
        {
            if (_selectedTable == null) return;

            _isEditMode = true;
            _editingTable = new TableViewModel
            {
                Id = _selectedTable.Id,
                TableNumber = _selectedTable.TableNumber,
                Capacity = _selectedTable.Capacity,
                Status = _selectedTable.Status,
                IsActive = _selectedTable.IsActive,
                ActiveReservationId = _selectedTable.ActiveReservationId
            };

            _tableDialogOpen = true;
        }

        private void SaveTable()
        {
            if (_isEditMode)
            {
                var table = _tables.First(t => t.Id == _editingTable.Id);
                table.TableNumber = _editingTable.TableNumber;
                table.Capacity = _editingTable.Capacity;
                table.Status = _editingTable.Status;
                table.IsActive = _editingTable.IsActive;
            }
            else
            {
                _tables.Add(new TableViewModel
                {
                    Id = Guid.NewGuid(),
                    TableNumber = _editingTable.TableNumber,
                    Capacity = _editingTable.Capacity,
                    Status = TableStatus.Available,
                    IsActive = true
                });
            }

            _tableDialogOpen = false;
        }

        private void OpenReserveTableDialog()
        {
            if (_selectedTable == null) return;

            _reservationCustomerName = string.Empty;
            _reservationPhoneNumber = string.Empty;
            _reservationDate = DateTime.Today;
            _reservationTime = new TimeSpan(19, 0, 0);
            _reservationTimeString = "19:00";
            _reservationGuestCount = 2;

            _reserveDialogOpen = true;
        }

        private void ConfirmReservation()
        {
            if (_selectedTable == null || string.IsNullOrWhiteSpace(_reservationCustomerName))
                return;

            _selectedTable.Status = TableStatus.Reserved;
            _selectedTable.ActiveReservationId = Guid.NewGuid();
            _selectedTable.CustomerName = _reservationCustomerName;
            _selectedTable.CustomerPhone = _reservationPhoneNumber;
            _selectedTable.ReservationDate = _reservationDate;
            _selectedTable.ReservationTime = _reservationTime;
            _selectedTable.GuestCount = _reservationGuestCount;

            _reserveDialogOpen = false;
        }

        private void CancelReservation(TableViewModel table)
        {
            table.Status = TableStatus.Available;
            table.ActiveReservationId = null;
            table.CustomerName = null;
            table.CustomerPhone = null;
            table.ReservationDate = null;
            table.ReservationTime = null;
            table.GuestCount = null;
        }

        private void OpenDeleteConfirmation()
        {
            if (_selectedTable?.Status == TableStatus.Available)
                _deleteDialogOpen = true;
        }

        private void DeleteTable()
        {
            if (_selectedTable == null) return;

            _tables.Remove(_selectedTable);
            _selectedTable = null;
            _deleteDialogOpen = false;
            _detailsDrawerOpen = false;
        }

        private void ToggleTableActive()
        {
            if (_selectedTable == null) return;
            _selectedTable.IsActive = !_selectedTable.IsActive;
        }

        private void ChangeTableStatus(TableViewModel table, TableStatus status)
        {
            table.Status = status;
        }

        private void OnTimeChanged(ChangeEventArgs e)
        {
            if (TimeSpan.TryParse(e.Value?.ToString(), out var time))
            {
                _reservationTime = time;
                _reservationTimeString = time.ToString(@"hh\:mm");
            }
        }

        // ================= UI Helpers =================
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

        private string GetTableCardClass(TableViewModel table)
        {
            var classes = new List<string>();

            if (_selectedTable?.Id == table.Id)
                classes.Add("table-card-selected");

            if (!table.IsActive)
                classes.Add("table-card-inactive");

            return string.Join(" ", classes);
        }
    }

    // ================= ViewModel =================
    public class TableViewModel
    {
        public Guid Id { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public TableStatus Status { get; set; }

        public Guid? ActiveReservationId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime? ReservationDate { get; set; }
        public TimeSpan? ReservationTime { get; set; }
        public int? GuestCount { get; set; }
    }

    public enum TableStatus
    {
        Available,
        Occupied,
        Reserved
    }
}
