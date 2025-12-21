using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.Dtos.Venues;
using Shared.Dtos.VenueTypes;
using Shared.Services.Venues;
using Shared.Services.VenueTypes;

namespace ReserveMe.Pages.Venues;

public partial class VenueSearch : ComponentBase
{
	[Inject] private IVenueTypesService _venueTypesService { get; set; } = null!;
	[Inject] private IVenuesService _venuesService { get; set; } = null!;
	[Inject] private IJSRuntime jsRuntime { get; set; }

	// Location
	private string _currentLocation = "Sofia, Bulgaria";
	private double _userLatitude = 42.3915;
	private double _userLongitude = 23.2111;
	private int _selectedRadius = 5;

	// Search and filters
	private string _searchTerm = string.Empty;
	private SortOption _sortBy = SortOption.Rating;
	private HashSet<int> _selectedTypeIds = new();
	private bool _filterModalOpen = false;

	// Data
	private List<VenueSearchDto> _venues = new();
	private List<VenueSearchDto> _visibleVenues = new();
	private List<VenueTypeDto> _venueTypes = new();
	private int _pageSize = 6;
	private int _currentPage = 1;
	private int _totalCount = 0;
	private bool _hasMore = true;

	// UI State
	private bool _isLoading = false;
	private bool _isLoadingMore = false;
	private string? _errorMessage;

	// Filtered venues
	private IEnumerable<VenueSearchDto> _filteredVenues => _visibleVenues;

	protected override async Task OnInitializedAsync()
	{
		_isLoading = true;
		try
		{
			_venues = await _venuesService.GetVenuesForClient();
			_venueTypes = await _venueTypesService.GetAllAsync();

			ResetPaging();
			FilterAndPaginate();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			_errorMessage = "Couldn't load venues at the moment. Please try again.";
		}
		finally
		{
			_isLoading = false;
		}
	}


	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender) return;

		try
		{
			var pos = await jsRuntime.InvokeAsync<GeoPos>("reserveMeGeo.getCurrentPosition", new
			{
				enableHighAccuracy = true,
				timeout = 10000,
				maximumAge = 60000
			});

			_userLatitude = pos.lat;
			_userLongitude = pos.lon;
			_currentLocation = "Near you";

			ResetPaging();
			FilterAndPaginate();
		}
		catch (JSException ex)
		{
			_errorMessage = "We couldn't access your location. Showing results for the default area.";
			StateHasChanged();
		}
	}

	private sealed class GeoPos
	{
		public double lat { get; set; }
		public double lon { get; set; }
		public double accuracy { get; set; }
	}

	private void OnSearchChanged()
	{
		ResetPaging();
		FilterAndPaginate();
	}

	private void OnFilterChanged()
	{
		ResetPaging();
		FilterAndPaginate();
	}

	private void LoadMore()
	{
		if (_isLoadingMore || !_hasMore) return;

		_isLoadingMore = true;
		try
		{
			_currentPage++;
			FilterAndPaginate();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		finally
		{
			_isLoadingMore = false;
		}
	}

	private void ClearSearch()
	{
		_searchTerm = string.Empty;
		ResetPaging();
		FilterAndPaginate();
	}

	private void ToggleFilterModal()
	{
		_filterModalOpen = !_filterModalOpen;
	}

	private void ToggleTypeFilter(int typeId)
	{
		if (_selectedTypeIds.Contains(typeId))
			_selectedTypeIds.Remove(typeId);
		else
			_selectedTypeIds.Add(typeId);
		StateHasChanged();
	}

	private void RemoveTypeFilter(int typeId)
	{
		_selectedTypeIds.Remove(typeId);
		ResetPaging();
		FilterAndPaginate();
	}

	private void ClearAllFilters()
	{
		_selectedTypeIds.Clear();
		_filterModalOpen = false;

		ResetPaging();
		FilterAndPaginate();
	}

	private void ApplyFilters()
	{
		_filterModalOpen = false;
		ResetPaging();
		FilterAndPaginate();
	}

	private void ToggleFavorite(VenueSearchDto venue)
	{
		try
		{
			throw new NotImplementedException();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private void ResetPaging()
	{
		_currentPage = 1;
		_visibleVenues.Clear();
		_hasMore = true;
	}

	private string FormatDistance(VenueSearchDto v)
	{
		var d = HaversineKm(_userLatitude, _userLongitude, v.Latitude, v.Longitude);
		return $"{d:0.0} km";
	}

	/// <summary>
	/// Applies search, type, radius, sorting and then paginates.
	/// </summary>
	private void FilterAndPaginate()
	{
		if (_venues is null) return;

		IEnumerable<VenueSearchDto> query = _venues;

		// 1) Search by name
		if (!string.IsNullOrWhiteSpace(_searchTerm))
		{
			var term = _searchTerm.Trim();
			query = query.Where(v =>
				!string.IsNullOrEmpty(v.Name) &&
				v.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
		}

		// 2) Filter by type
		if (_selectedTypeIds.Any())
		{
			query = query.Where(v => v.VenueTypeId.HasValue && _selectedTypeIds.Contains(v.VenueTypeId.Value));
		}

		// 3) Radius filter
		query = query.Where(v =>
		{
			var d = HaversineKm(_userLatitude, _userLongitude, v.Latitude, v.Longitude);
			return d <= _selectedRadius;
		});

		// 4) Sort
		query = _sortBy switch
		{
			SortOption.Rating => query
				.OrderByDescending(v => v.Rating ?? 0)
				.ThenByDescending(v => v.ReviewCount),
			SortOption.Reservations => query
				.OrderByDescending(v => v.TotalReservations)
				.ThenByDescending(v => v.Rating ?? 0),
			_ => query
		};

		// 5) Count AFTER filters but BEFORE pagination
		_totalCount = query.Count();

		// 6) Pagination (show N pages worth of items)
		var take = _currentPage * _pageSize;
		_visibleVenues = query.Take(take).ToList();

		_hasMore = _visibleVenues.Count < _totalCount;
		StateHasChanged();
	}

	private string GetVenueTypeName(int? typeId)
	{
		if (!typeId.HasValue) return "Other";
		var type = _venueTypes.FirstOrDefault(t => t.Id == typeId);
		return type?.Name ?? "Other";
	}

	private static string GetVenueTypeIcon(int? typeId) => typeId switch
	{
		1 => "🍽️", // Restaurant
		2 => "🍸", // Bar
		3 => "☕", // Cafe
		4 => "🔥", // Grill
		5 => "🍕", // Pizzeria
		6 => "🍔", // Fast Food
		7 => "🍺", // Pub
		8 => "🥗", // Bistro
		_ => "📍"
	};

	// If you want to compute distances client-side:
	private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
	{
		const double R = 6371.0; // km
		double dLat = Deg2Rad(lat2 - lat1);
		double dLon = Deg2Rad(lon2 - lon1);
		double a =
			Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
			Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
			Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
		double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
		return R * c;
	}

	private static double Deg2Rad(double deg) => deg * Math.PI / 180.0;
}

public enum SortOption
{
	Rating,
	Reservations
}