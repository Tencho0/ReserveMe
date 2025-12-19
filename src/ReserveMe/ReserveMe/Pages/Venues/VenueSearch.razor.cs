using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Shared.Dtos;

namespace ReserveMe.Pages.Venues;

public partial class VenueSearch : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    // Location
    private string _currentLocation = "Sofia, Bulgaria";
    private double _userLatitude = 42.6977;
    private double _userLongitude = 23.3219;
    private int _selectedRadius = 5;

    // Search and filters
    private string _searchTerm = string.Empty;
    private SortOption _sortBy = SortOption.Rating;
    private HashSet<int> _selectedTypeIds = new();
    private bool _filterModalOpen = false;

    // Data
    private List<VenueViewModel> _venues = new();
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
    private IEnumerable<VenueViewModel> _filteredVenues => _venues;

    protected override async Task OnInitializedAsync()
    {
        await LoadVenueTypes();
        await LoadVenues();
    }

    private async Task LoadVenueTypes()
    {
        try
        {
            var types = await Http.GetFromJsonAsync<List<VenueTypeDto>>("api/venues/types");
            _venueTypes = types ?? new List<VenueTypeDto>();
        }
        catch
        {
            // Use default types if API fails
            _venueTypes = new List<VenueTypeDto>
            {
                new() { Id = 1, Name = "Restaurant" },
                new() { Id = 2, Name = "Bar" },
                new() { Id = 3, Name = "Cafe" },
                new() { Id = 4, Name = "Grill" },
                new() { Id = 5, Name = "Pizzeria" },
                new() { Id = 6, Name = "Fast Food" },
                new() { Id = 7, Name = "Pub" },
                new() { Id = 8, Name = "Bistro" }
            };
        }
    }

    private async Task LoadVenues()
    {
        _isLoading = true;
        _errorMessage = null;
        StateHasChanged();

        try
        {
            var sortByString = _sortBy switch
            {
                SortOption.Rating => "rating",
                SortOption.Distance => "distance",
                SortOption.Reservations => "reservations",
                _ => "rating"
            };

            var typeIdParam = _selectedTypeIds.Any() ? string.Join("", _selectedTypeIds.Select(id => $"&typeIds={id}")) : "";
            var searchParam = !string.IsNullOrWhiteSpace(_searchTerm) ? $"&search={Uri.EscapeDataString(_searchTerm)}" : "";

            var lat = _userLatitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lon = _userLongitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"api/venues/search?latitude={lat}&longitude={lon}&radiusKm={_selectedRadius}&sortBy={sortByString}&page={_currentPage}&pageSize={_pageSize}{typeIdParam}{searchParam}";

            var result = await Http.GetFromJsonAsync<VenueSearchResultDto>(url);

            if (result != null)
            {
                _venues = result.Venues.Select(MapToViewModel).ToList();
                _totalCount = result.TotalCount;
                _hasMore = result.HasMore;
                _currentPage = 1;
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error loading venues: {ex.Message}";
            // Filter mock data by radius and apply pagination
            var allMockVenues = GetAllMockVenues();
            var filtered = allMockVenues
                .Where(v => v.Distance <= _selectedRadius)
                .ToList();

            // Apply sorting
            filtered = _sortBy switch
            {
                SortOption.Rating => filtered.OrderByDescending(v => v.Rating).ToList(),
                SortOption.Distance => filtered.OrderBy(v => v.Distance).ToList(),
                SortOption.Reservations => filtered.OrderByDescending(v => v.TotalReservations).ToList(),
                _ => filtered
            };

            _totalCount = filtered.Count;
            _venues = filtered.Take(_pageSize).ToList();
            _hasMore = filtered.Count > _pageSize;
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadMore()
    {
        if (_isLoadingMore || !_hasMore) return;

        _isLoadingMore = true;
        StateHasChanged();

        try
        {
            _currentPage++;

            var sortByString = _sortBy switch
            {
                SortOption.Rating => "rating",
                SortOption.Distance => "distance",
                SortOption.Reservations => "reservations",
                _ => "rating"
            };

            var typeIdParam = _selectedTypeIds.Any() ? $"&typeIds={_selectedTypeIds.First()}" : "";
            var searchParam = !string.IsNullOrWhiteSpace(_searchTerm) ? $"&search={Uri.EscapeDataString(_searchTerm)}" : "";

            var lat = _userLatitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lon = _userLongitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"api/venues/search?latitude={lat}&longitude={lon}&radiusKm={_selectedRadius}&sortBy={sortByString}&page={_currentPage}&pageSize={_pageSize}{typeIdParam}{searchParam}";

            var result = await Http.GetFromJsonAsync<VenueSearchResultDto>(url);

            if (result != null && result.Venues.Any())
            {
                _venues.AddRange(result.Venues.Select(MapToViewModel));
                _totalCount = result.TotalCount;
                _hasMore = result.HasMore;
            }
            else
            {
                _hasMore = false;
            }
        }
        catch
        {
            // Load more from mock data
            var allMockVenues = GetAllMockVenues()
                .Where(v => v.Distance <= _selectedRadius)
                .ToList();

            allMockVenues = _sortBy switch
            {
                SortOption.Rating => allMockVenues.OrderByDescending(v => v.Rating).ToList(),
                SortOption.Distance => allMockVenues.OrderBy(v => v.Distance).ToList(),
                SortOption.Reservations => allMockVenues.OrderByDescending(v => v.TotalReservations).ToList(),
                _ => allMockVenues
            };

            var newVenues = allMockVenues
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            if (newVenues.Any())
            {
                _venues.AddRange(newVenues);
                _hasMore = _venues.Count < allMockVenues.Count;
            }
            else
            {
                _hasMore = false;
            }
        }
        finally
        {
            _isLoadingMore = false;
            StateHasChanged();
        }
    }

    private async Task OnSearchChanged()
    {
        await LoadVenues();
    }

    private async Task OnFilterChanged()
    {
        await LoadVenues();
    }

    private async Task ClearSearch()
    {
        _searchTerm = string.Empty;
        await LoadVenues();
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
        _ = LoadVenues();
    }

    private async Task ClearAllFilters()
    {
        _selectedTypeIds.Clear();
        _filterModalOpen = false;
        await LoadVenues();
    }

    private async Task ApplyFilters()
    {
        _filterModalOpen = false;
        await LoadVenues();
    }

    private async Task ToggleFavorite(VenueViewModel venue)
    {
        try
        {
            var response = await Http.PostAsync($"api/venues/{venue.Id}/favorite", null);
            if (response.IsSuccessStatusCode)
            {
                venue.IsFavorite = !venue.IsFavorite;
                StateHasChanged();
            }
        }
        catch
        {
            // Fallback - just toggle locally
            venue.IsFavorite = !venue.IsFavorite;
            StateHasChanged();
        }
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

    private static VenueViewModel MapToViewModel(VenueSearchDto dto)
    {
        return new VenueViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            VenueTypeId = dto.VenueTypeId,
            VenueTypeName = dto.VenueTypeName,
            Rating = dto.Rating,
            ReviewCount = dto.ReviewCount,
            Distance = dto.Distance,
            Address = dto.Address ?? "Address not available",
            TotalReservations = dto.TotalReservations,
            ImageUrl = dto.ImageUrl ?? "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=400",
            LogoUrl = dto.LogoUrl,
            IsFavorite = dto.IsFavorite
        };
    }

    // Mock data (fallback)
    private List<VenueViewModel> GetAllMockVenues()
    {
        return new List<VenueViewModel>
        {
            new()
            {
                Id = 1,
                Name = "La Piazza",
                VenueTypeId = 1,
                VenueTypeName = "Restaurant",
                Rating = 4.8,
                ReviewCount = 245,
                Distance = 0.5,
                Address = "Graf Ignatiev St 15",
                TotalReservations = 1250,
                ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=100&h=100&fit=crop",
                IsFavorite = true
            },
            new()
            {
                Id = 2,
                Name = "The Irish Pub",
                VenueTypeId = 7,
                VenueTypeName = "Pub",
                Rating = 4.5,
                ReviewCount = 189,
                Distance = 1.2,
                Address = "Vitosha Blvd 88",
                TotalReservations = 890,
                ImageUrl = "https://images.unsplash.com/photo-1514933651103-005eec06c04b?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1541167760496-1628856ab772?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = 3,
                Name = "Grill House",
                VenueTypeId = 4,
                VenueTypeName = "Grill",
                Rating = 4.7,
                ReviewCount = 312,
                Distance = 2.1,
                Address = "Solunska St 22",
                TotalReservations = 2100,
                ImageUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = 4,
                Name = "Cafe Milano",
                VenueTypeId = 3,
                VenueTypeName = "Cafe",
                Rating = 4.3,
                ReviewCount = 156,
                Distance = 0.8,
                Address = "Slaveykov Sq 5",
                TotalReservations = 450,
                ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1511920170033-f8396924c348?w=100&h=100&fit=crop",
                IsFavorite = true
            },
            new()
            {
                Id = 5,
                Name = "Pizza Napoli",
                VenueTypeId = 5,
                VenueTypeName = "Pizzeria",
                Rating = 4.6,
                ReviewCount = 278,
                Distance = 1.5,
                Address = "Rakovski St 100",
                TotalReservations = 1800,
                ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = 6,
                Name = "Sky Bar",
                VenueTypeId = 2,
                VenueTypeName = "Bar",
                Rating = 4.9,
                ReviewCount = 420,
                Distance = 3.2,
                Address = "Cherni Vrah Blvd 47",
                TotalReservations = 3200,
                ImageUrl = "https://images.unsplash.com/photo-1470337458703-46ad1756a187?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1560512823-829485b8bf24?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = 7,
                Name = "Burger Joint",
                VenueTypeId = 6,
                VenueTypeName = "Fast Food",
                Rating = 3.8,
                ReviewCount = 520,
                Distance = 0.3,
                Address = "Maria Luiza Blvd 2",
                TotalReservations = 5600,
                ImageUrl = "https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1550547660-d9450f859349?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = 8,
                Name = "Green Bistro",
                VenueTypeId = 8,
                VenueTypeName = "Bistro",
                Rating = 4.4,
                ReviewCount = 98,
                Distance = 1.8,
                Address = "Shipka St 34",
                TotalReservations = 320,
                ImageUrl = "https://images.unsplash.com/photo-1466978913421-dad2ebd01d17?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = 9,
                Name = "Grandpa's Tavern",
                VenueTypeId = 1,
                VenueTypeName = "Restaurant",
                Rating = 4.7,
                ReviewCount = 356,
                Distance = 4.5,
                Address = "Bistritsa Village, Glavna St 1",
                TotalReservations = 980,
                ImageUrl = "https://images.unsplash.com/photo-1552566626-52f8b828add9?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=100&h=100&fit=crop",
                IsFavorite = true
            },
            new()
            {
                Id = 10,
                Name = "Cocktail Bar Neon",
                VenueTypeId = 2,
                VenueTypeName = "Bar",
                Rating = 4.2,
                ReviewCount = 145,
                Distance = 2.8,
                Address = "Tsar Shishman St 12",
                TotalReservations = 670,
                ImageUrl = "https://images.unsplash.com/photo-1572116469696-31de0f17cc34?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=100&h=100&fit=crop",
                IsFavorite = false
            }
        };
    }
}

// ViewModel
public class VenueViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? VenueTypeId { get; set; }
    public string? VenueTypeName { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public double Distance { get; set; }
    public string Address { get; set; } = string.Empty;
    public int TotalReservations { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsFavorite { get; set; }
}

public enum SortOption
{
    Rating,
    Distance,
    Reservations
}