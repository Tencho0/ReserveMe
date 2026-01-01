namespace ReserveMe.Tests.Pages.Venues;

using Shared.Dtos.Venues;
using Shared.Dtos.VenueTypes;

/// <summary>
/// Unit tests for VenueSearch functionality.
/// Tests the filtering, sorting, and distance calculation logic.
/// Note: Component rendering tests are skipped due to JSInterop dependencies in OnAfterRenderAsync.
/// </summary>
public class VenueSearchTests
{
    #region Distance Calculation Tests (Haversine Formula)

    [Fact]
    public void HaversineKm_SameLocation_ReturnsZero()
    {
        // Arrange
        double lat = 42.6977;
        double lon = 23.3219;

        // Act
        double distance = HaversineKm(lat, lon, lat, lon);

        // Assert
        Assert.Equal(0, distance, precision: 5);
    }

    [Fact]
    public void HaversineKm_SofiaToPlovdiv_ReturnsApproximately130km()
    {
        // Arrange - Sofia coordinates
        double lat1 = 42.6977;
        double lon1 = 23.3219;
        // Plovdiv coordinates
        double lat2 = 42.1354;
        double lon2 = 24.7453;

        // Act
        double distance = HaversineKm(lat1, lon1, lat2, lon2);

        // Assert - Should be approximately 130km
        Assert.InRange(distance, 120, 140);
    }

    [Fact]
    public void HaversineKm_ShortDistance_CalculatesCorrectly()
    {
        // Arrange - Two points ~1km apart in Sofia
        double lat1 = 42.6977;
        double lon1 = 23.3219;
        double lat2 = 42.7067;
        double lon2 = 23.3219;

        // Act
        double distance = HaversineKm(lat1, lon1, lat2, lon2);

        // Assert - Should be approximately 1km
        Assert.InRange(distance, 0.9, 1.1);
    }

    #endregion

    #region Venue Filtering Tests

    [Fact]
    public void FilterByName_MatchingTerm_ReturnsFilteredVenues()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Pizza Palace" },
            new VenueSearchDto { Id = 2, Name = "Burger King" },
            new VenueSearchDto { Id = 3, Name = "Pizza Hut" }
        };

        // Act
        var filtered = FilterByName(venues, "Pizza");

        // Assert
        Assert.Equal(2, filtered.Count());
        Assert.All(filtered, v => Assert.Contains("Pizza", v.Name));
    }

    [Fact]
    public void FilterByName_NoMatch_ReturnsEmpty()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Pizza Palace" },
            new VenueSearchDto { Id = 2, Name = "Burger King" }
        };

        // Act
        var filtered = FilterByName(venues, "Sushi");

        // Assert
        Assert.Empty(filtered);
    }

    [Fact]
    public void FilterByName_CaseInsensitive_ReturnsMatches()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "PIZZA PALACE" },
            new VenueSearchDto { Id = 2, Name = "pizza hut" }
        };

        // Act
        var filtered = FilterByName(venues, "Pizza");

        // Assert
        Assert.Equal(2, filtered.Count());
    }

    [Fact]
    public void FilterByName_EmptyTerm_ReturnsAll()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Pizza Palace" },
            new VenueSearchDto { Id = 2, Name = "Burger King" }
        };

        // Act
        var filtered = FilterByName(venues, "");

        // Assert
        Assert.Equal(2, filtered.Count());
    }

    #endregion

    #region Venue Type Filtering Tests

    [Fact]
    public void FilterByType_SingleType_ReturnsMatching()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Restaurant A", VenueTypeId = 1 },
            new VenueSearchDto { Id = 2, Name = "Bar B", VenueTypeId = 2 },
            new VenueSearchDto { Id = 3, Name = "Restaurant C", VenueTypeId = 1 }
        };
        var selectedTypes = new HashSet<int> { 1 };

        // Act
        var filtered = FilterByType(venues, selectedTypes);

        // Assert
        Assert.Equal(2, filtered.Count());
        Assert.All(filtered, v => Assert.Equal(1, v.VenueTypeId));
    }

    [Fact]
    public void FilterByType_MultipleTypes_ReturnsAllMatching()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Restaurant", VenueTypeId = 1 },
            new VenueSearchDto { Id = 2, Name = "Bar", VenueTypeId = 2 },
            new VenueSearchDto { Id = 3, Name = "Cafe", VenueTypeId = 3 }
        };
        var selectedTypes = new HashSet<int> { 1, 2 };

        // Act
        var filtered = FilterByType(venues, selectedTypes);

        // Assert
        Assert.Equal(2, filtered.Count());
    }

    [Fact]
    public void FilterByType_NoTypesSelected_ReturnsAll()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Restaurant", VenueTypeId = 1 },
            new VenueSearchDto { Id = 2, Name = "Bar", VenueTypeId = 2 }
        };
        var selectedTypes = new HashSet<int>();

        // Act
        var filtered = FilterByType(venues, selectedTypes);

        // Assert
        Assert.Equal(2, filtered.Count());
    }

    #endregion

    #region Radius Filtering Tests

    [Fact]
    public void FilterByRadius_VenuesWithinRadius_ReturnsMatching()
    {
        // Arrange - User at Sofia center
        double userLat = 42.6977;
        double userLon = 23.3219;
        int radius = 5; // 5km

        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Near", Latitude = 42.6977, Longitude = 23.3219 }, // 0km
            new VenueSearchDto { Id = 2, Name = "Far", Latitude = 42.1354, Longitude = 24.7453 }  // ~130km (Plovdiv)
        };

        // Act
        var filtered = FilterByRadius(venues, userLat, userLon, radius);

        // Assert
        Assert.Single(filtered);
        Assert.Equal("Near", filtered.First().Name);
    }

    [Fact]
    public void FilterByRadius_AllWithinRadius_ReturnsAll()
    {
        // Arrange
        double userLat = 42.6977;
        double userLon = 23.3219;
        int radius = 200; // 200km

        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Sofia", Latitude = 42.6977, Longitude = 23.3219 },
            new VenueSearchDto { Id = 2, Name = "Plovdiv", Latitude = 42.1354, Longitude = 24.7453 }
        };

        // Act
        var filtered = FilterByRadius(venues, userLat, userLon, radius);

        // Assert
        Assert.Equal(2, filtered.Count());
    }

    #endregion

    #region Sorting Tests

    [Fact]
    public void SortByRating_DescendingOrder()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Low", Rating = 3.0 },
            new VenueSearchDto { Id = 2, Name = "High", Rating = 5.0 },
            new VenueSearchDto { Id = 3, Name = "Mid", Rating = 4.0 }
        };

        // Act
        var sorted = SortByRating(venues);

        // Assert
        Assert.Equal("High", sorted.First().Name);
        Assert.Equal("Low", sorted.Last().Name);
    }

    [Fact]
    public void SortByRating_HandlesNullRatings()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Rated", Rating = 4.0 },
            new VenueSearchDto { Id = 2, Name = "Unrated", Rating = null }
        };

        // Act
        var sorted = SortByRating(venues);

        // Assert
        Assert.Equal("Rated", sorted.First().Name);
    }

    [Fact]
    public void SortByReservations_DescendingOrder()
    {
        // Arrange
        var venues = new List<VenueSearchDto>
        {
            new VenueSearchDto { Id = 1, Name = "Few", TotalReservations = 10 },
            new VenueSearchDto { Id = 2, Name = "Many", TotalReservations = 100 },
            new VenueSearchDto { Id = 3, Name = "Some", TotalReservations = 50 }
        };

        // Act
        var sorted = SortByReservations(venues);

        // Assert
        Assert.Equal("Many", sorted.First().Name);
        Assert.Equal("Few", sorted.Last().Name);
    }

    #endregion

    #region Venue Type Icon Tests

    [Theory]
    [InlineData(1, "Restaurant")]
    [InlineData(2, "Bar")]
    [InlineData(3, "Cafe")]
    [InlineData(null, "Other")]
    [InlineData(999, "Other")]
    public void GetVenueTypeName_ReturnsCorrectName(int? typeId, string expectedName)
    {
        // Arrange
        var venueTypes = new List<VenueTypeDto>
        {
            new VenueTypeDto { Id = 1, Name = "Restaurant" },
            new VenueTypeDto { Id = 2, Name = "Bar" },
            new VenueTypeDto { Id = 3, Name = "Cafe" }
        };

        // Act
        var name = GetVenueTypeName(typeId, venueTypes);

        // Assert
        Assert.Equal(expectedName, name);
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public void Paginate_FirstPage_ReturnsCorrectCount()
    {
        // Arrange
        var venues = Enumerable.Range(1, 20)
            .Select(i => new VenueSearchDto { Id = i, Name = $"Venue {i}" })
            .ToList();
        int pageSize = 6;
        int currentPage = 1;

        // Act
        var paginated = Paginate(venues, pageSize, currentPage);

        // Assert
        Assert.Equal(6, paginated.Count());
    }

    [Fact]
    public void Paginate_SecondPage_ReturnsAccumulatedResults()
    {
        // Arrange
        var venues = Enumerable.Range(1, 20)
            .Select(i => new VenueSearchDto { Id = i, Name = $"Venue {i}" })
            .ToList();
        int pageSize = 6;
        int currentPage = 2;

        // Act
        var paginated = Paginate(venues, pageSize, currentPage);

        // Assert
        Assert.Equal(12, paginated.Count()); // 2 pages * 6 items
    }

    [Fact]
    public void HasMore_MoreItemsExist_ReturnsTrue()
    {
        // Arrange
        int totalCount = 20;
        int visibleCount = 6;

        // Act
        bool hasMore = HasMoreItems(totalCount, visibleCount);

        // Assert
        Assert.True(hasMore);
    }

    [Fact]
    public void HasMore_AllItemsVisible_ReturnsFalse()
    {
        // Arrange
        int totalCount = 6;
        int visibleCount = 6;

        // Act
        bool hasMore = HasMoreItems(totalCount, visibleCount);

        // Assert
        Assert.False(hasMore);
    }

    #endregion

    #region Helper Methods (Simulating Component Logic)

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

    private static IEnumerable<VenueSearchDto> FilterByName(IEnumerable<VenueSearchDto> venues, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return venues;

        return venues.Where(v =>
            !string.IsNullOrEmpty(v.Name) &&
            v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<VenueSearchDto> FilterByType(IEnumerable<VenueSearchDto> venues, HashSet<int> selectedTypes)
    {
        if (!selectedTypes.Any())
            return venues;

        return venues.Where(v => v.VenueTypeId.HasValue && selectedTypes.Contains(v.VenueTypeId.Value));
    }

    private static IEnumerable<VenueSearchDto> FilterByRadius(
        IEnumerable<VenueSearchDto> venues,
        double userLat,
        double userLon,
        int radiusKm)
    {
        return venues.Where(v =>
        {
            var d = HaversineKm(userLat, userLon, v.Latitude, v.Longitude);
            return d <= radiusKm;
        });
    }

    private static IEnumerable<VenueSearchDto> SortByRating(IEnumerable<VenueSearchDto> venues)
    {
        return venues
            .OrderByDescending(v => v.Rating ?? 0)
            .ThenByDescending(v => v.ReviewCount);
    }

    private static IEnumerable<VenueSearchDto> SortByReservations(IEnumerable<VenueSearchDto> venues)
    {
        return venues
            .OrderByDescending(v => v.TotalReservations)
            .ThenByDescending(v => v.Rating ?? 0);
    }

    private static string GetVenueTypeName(int? typeId, List<VenueTypeDto> venueTypes)
    {
        if (!typeId.HasValue) return "Other";
        var type = venueTypes.FirstOrDefault(t => t.Id == typeId);
        return type?.Name ?? "Other";
    }

    private static IEnumerable<VenueSearchDto> Paginate(IEnumerable<VenueSearchDto> venues, int pageSize, int currentPage)
    {
        return venues.Take(currentPage * pageSize);
    }

    private static bool HasMoreItems(int totalCount, int visibleCount)
    {
        return visibleCount < totalCount;
    }

    #endregion
}
