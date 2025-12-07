using Microsoft.AspNetCore.Components;

namespace ReserveMe.Pages.Venues;

public partial class VenueSearch : ComponentBase
{
    // Location
    private string _currentLocation = "София, ул. Витоша 50";
    private int _selectedRadius = 5;

    // Search and filters
    private string _searchTerm = string.Empty;
    private SortOption _sortBy = SortOption.Rating;
    private HashSet<VenueType> _selectedTypes = new();
    private bool _filterModalOpen = false;

    // Data
    private List<VenueViewModel> _venues = new();
    private int _pageSize = 6;
    private int _currentPage = 1;
    private int _totalCount = 0;
    private bool _hasMore = true;

    // UI State
    private bool _isLoading = false;
    private bool _isLoadingMore = false;

    // Filtered venues
    private IEnumerable<VenueViewModel> _filteredVenues => GetFilteredAndSortedVenues();

    protected override async Task OnInitializedAsync()
    {
        await LoadVenues();
    }

    private async Task LoadVenues()
    {
        _isLoading = true;
        StateHasChanged();

        // Simulate API call
        await Task.Delay(500);
        _venues = GetMockVenues();
        _totalCount = _venues.Count;
        _hasMore = _venues.Count >= _pageSize;

        _isLoading = false;
        StateHasChanged();
    }

    private async Task LoadMore()
    {
        if (_isLoadingMore || !_hasMore) return;

        _isLoadingMore = true;
        StateHasChanged();

        await Task.Delay(500);

        _currentPage++;
        var newVenues = GetMockVenues(_currentPage);

        if (newVenues.Any())
        {
            _venues.AddRange(newVenues);
            _totalCount = _venues.Count;
        }

        _hasMore = newVenues.Count >= _pageSize;
        _isLoadingMore = false;
        StateHasChanged();
    }

    private void OnSearchChanged()
    {
        StateHasChanged();
    }

    private void OnFilterChanged()
    {
        StateHasChanged();
    }

    private void ClearSearch()
    {
        _searchTerm = string.Empty;
        StateHasChanged();
    }

    private IEnumerable<VenueViewModel> GetFilteredAndSortedVenues()
    {
        var query = _venues.AsEnumerable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(_searchTerm))
        {
            var term = _searchTerm.ToLower();
            query = query.Where(v => v.Name.ToLower().Contains(term));
        }

        // Type filter
        if (_selectedTypes.Any())
        {
            query = query.Where(v => _selectedTypes.Contains(v.Type));
        }

        // Radius filter
        query = query.Where(v => v.Distance <= _selectedRadius);

        // Sorting
        query = _sortBy switch
        {
            SortOption.Rating => query.OrderByDescending(v => v.Rating),
            SortOption.Distance => query.OrderBy(v => v.Distance),
            SortOption.Reservations => query.OrderByDescending(v => v.TotalReservations),
            _ => query.OrderByDescending(v => v.Rating)
        };

        return query;
    }

    private void ToggleFilterModal()
    {
        _filterModalOpen = !_filterModalOpen;
    }

    private void ToggleTypeFilter(VenueType type)
    {
        if (_selectedTypes.Contains(type))
            _selectedTypes.Remove(type);
        else
            _selectedTypes.Add(type);
        StateHasChanged();
    }

    private void RemoveTypeFilter(VenueType type)
    {
        _selectedTypes.Remove(type);
        StateHasChanged();
    }

    private void ClearAllFilters()
    {
        _selectedTypes.Clear();
        _filterModalOpen = false;
        StateHasChanged();
    }

    private void ApplyFilters()
    {
        _filterModalOpen = false;
        StateHasChanged();
    }

    private void ToggleFavorite(VenueViewModel venue)
    {
        venue.IsFavorite = !venue.IsFavorite;
        StateHasChanged();
    }

    private static string GetVenueTypeName(VenueType type) => type switch
    {
        VenueType.Restaurant => "Ресторант",
        VenueType.Bar => "Бар",
        VenueType.Cafe => "Кафене",
        VenueType.Grill => "Скара",
        VenueType.Pizzeria => "Пицария",
        VenueType.FastFood => "Бързо хранене",
        VenueType.Pub => "Пъб",
        VenueType.Bistro => "Бистро",
        _ => "Друго"
    };

    private static string GetVenueTypeIcon(VenueType type) => type switch
    {
        VenueType.Restaurant => "🍽️",
        VenueType.Bar => "🍸",
        VenueType.Cafe => "☕",
        VenueType.Grill => "🔥",
        VenueType.Pizzeria => "🍕",
        VenueType.FastFood => "🍔",
        VenueType.Pub => "🍺",
        VenueType.Bistro => "🥗",
        _ => "📍"
    };

    // Mock data
    private List<VenueViewModel> GetMockVenues(int page = 1)
    {
        var allVenues = new List<VenueViewModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "La Piazza",
                Type = VenueType.Restaurant,
                Rating = 4.8,
                ReviewCount = 245,
                Distance = 0.5,
                Address = "ул. Граф Игнатиев 15",
                TotalReservations = 1250,
                ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=100&h=100&fit=crop",
                IsFavorite = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "The Irish Pub",
                Type = VenueType.Pub,
                Rating = 4.5,
                ReviewCount = 189,
                Distance = 1.2,
                Address = "бул. Витоша 88",
                TotalReservations = 890,
                ImageUrl = "https://images.unsplash.com/photo-1514933651103-005eec06c04b?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1541167760496-1628856ab772?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Grill House",
                Type = VenueType.Grill,
                Rating = 4.7,
                ReviewCount = 312,
                Distance = 2.1,
                Address = "ул. Солунска 22",
                TotalReservations = 2100,
                ImageUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Cafe Milano",
                Type = VenueType.Cafe,
                Rating = 4.3,
                ReviewCount = 156,
                Distance = 0.8,
                Address = "пл. Славейков 5",
                TotalReservations = 450,
                ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1511920170033-f8396924c348?w=100&h=100&fit=crop",
                IsFavorite = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Pizza Napoli",
                Type = VenueType.Pizzeria,
                Rating = 4.6,
                ReviewCount = 278,
                Distance = 1.5,
                Address = "ул. Раковски 100",
                TotalReservations = 1800,
                ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Sky Bar",
                Type = VenueType.Bar,
                Rating = 4.9,
                ReviewCount = 420,
                Distance = 3.2,
                Address = "бул. Черни връх 47",
                TotalReservations = 3200,
                ImageUrl = "https://images.unsplash.com/photo-1470337458703-46ad1756a187?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1560512823-829485b8bf24?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Burger Joint",
                Type = VenueType.FastFood,
                Rating = 3.8,
                ReviewCount = 520,
                Distance = 0.3,
                Address = "бул. Мария Луиза 2",
                TotalReservations = 5600,
                ImageUrl = "https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1550547660-d9450f859349?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Green Bistro",
                Type = VenueType.Bistro,
                Rating = 4.4,
                ReviewCount = 98,
                Distance = 1.8,
                Address = "ул. Шипка 34",
                TotalReservations = 320,
                ImageUrl = "https://images.unsplash.com/photo-1466978913421-dad2ebd01d17?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=100&h=100&fit=crop",
                IsFavorite = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Механа При Дядо",
                Type = VenueType.Restaurant,
                Rating = 4.7,
                ReviewCount = 356,
                Distance = 4.5,
                Address = "с. Бистрица, ул. Главна 1",
                TotalReservations = 980,
                ImageUrl = "https://images.unsplash.com/photo-1552566626-52f8b828add9?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=100&h=100&fit=crop",
                IsFavorite = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Cocktail Bar Neon",
                Type = VenueType.Bar,
                Rating = 4.2,
                ReviewCount = 145,
                Distance = 2.8,
                Address = "ул. Цар Шишман 12",
                TotalReservations = 670,
                ImageUrl = "https://images.unsplash.com/photo-1572116469696-31de0f17cc34?w=400",
                LogoUrl = "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=100&h=100&fit=crop",
                IsFavorite = false
            },
        };

        var skip = (page - 1) * _pageSize;
        return allVenues.Skip(skip).Take(_pageSize).ToList();
    }
}

// ViewModels and Enums
public class VenueViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public VenueType Type { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public double Distance { get; set; }
    public string Address { get; set; } = string.Empty;
    public int TotalReservations { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
}

public enum VenueType
{
    Restaurant,
    Bar,
    Cafe,
    Grill,
    Pizzeria,
    FastFood,
    Pub,
    Bistro
}

public enum SortOption
{
    Rating,
    Distance,
    Reservations
}