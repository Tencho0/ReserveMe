using Common.Enums;
using Microsoft.AspNetCore.Components;
using Shared.Dtos.Reservations;
using Shared.Helpers;
using Shared.Services.Reservations;

namespace ReserveMe.Pages.Owner;

public partial class ReservationManagement : ComponentBase
{
	[Inject] private IReservationsService _reservationsService { get; set; } = null!;

	[Inject] private IAuthenticationHelper _authHelper { get; set; } = null!;

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
	private List<ReservationDto> _reservations = new();
	private ReservationDto? _selectedReservation;
	private bool _showDetailsDialog = false;

	// Filtered and paged data
	private IEnumerable<ReservationDto> _filteredReservations => FilterReservations();
	private IEnumerable<ReservationDto> _pagedReservations => _filteredReservations
		.Skip((_currentPage - 1) * _pageSize)
		.Take(_pageSize);

	private static DateTime GetReservationDate(ReservationDto r) => (r.ReservationTime ?? DateTime.MinValue).Date;
	private static TimeSpan GetReservationTimeOfDay(ReservationDto r) => (r.ReservationTime ?? DateTime.MinValue).TimeOfDay;

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
		int venueId = await _authHelper.GetUserMenuId();

		_reservations = await _reservationsService.GetReservations(venueId);
	}

	private IEnumerable<ReservationDto> FilterReservations()
	{
		var query = _reservations.AsEnumerable();

		if (!string.IsNullOrWhiteSpace(_searchTerm))
		{
			var term = _searchTerm.ToLower();
			query = query.Where(r =>
				r.ContactName.ToLower().Contains(term) ||
				r.ContactPhone.Contains(term));
		}

		if (_dateFrom.HasValue)
			query = query.Where(r => GetReservationDate(r) >= _dateFrom.Value.Date);

		if (_dateTo.HasValue)
			query = query.Where(r => GetReservationDate(r) <= _dateTo.Value.Date);

		if (_timeFrom.HasValue)
			query = query.Where(r => GetReservationTimeOfDay(r) >= _timeFrom.Value);

		if (_timeTo.HasValue)
			query = query.Where(r => GetReservationTimeOfDay(r) <= _timeTo.Value);

		if (_statusFilter.HasValue)
			query = query.Where(r => r.Status == _statusFilter.Value);

		if (_tableNumberFilter.HasValue)
			query = query.Where(r => r.TableNumber == _tableNumberFilter.Value);

		return query
			.OrderBy(GetReservationDate)
			.ThenBy(GetReservationTimeOfDay);
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

	private async void ChangeStatus(ReservationDto reservation, ReservationStatus newStatus)
	{
		reservation.Status = newStatus;
		await _reservationsService.ChangeReservationStatus(reservation.Id, newStatus);
		StateHasChanged();
	}

	private void ViewDetails(ReservationDto reservation)
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
}