namespace ReserveMe.Tests.Pages.Venues;

using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using ReserveMe.Pages.Venues;
using Shared.Dtos.Reviews;
using Shared.Dtos.Venues;
using Shared.Dtos.VenueTypes;
using Shared.Helpers;
using Shared.Services.Reviews;
using Shared.Services.Venues;

/// <summary>
/// Unit tests for the VenueDetails page component.
/// Tests cover venue info display, reviews, and reservation links.
/// </summary>
public class VenueDetailsTests : TestContext
{
    private readonly Mock<IVenuesService> _venuesServiceMock;
    private readonly Mock<IReviewsService> _reviewsServiceMock;
    private readonly Mock<IAuthenticationHelper> _authHelperMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public VenueDetailsTests()
    {
        _venuesServiceMock = new Mock<IVenuesService>();
        _reviewsServiceMock = new Mock<IReviewsService>();
        _authHelperMock = new Mock<IAuthenticationHelper>();
        _configurationMock = new Mock<IConfiguration>();

        // Default setup
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync("user-123");

        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test Restaurant",
                Description = "A lovely place",
                Rating = 4.5,
                ReviewCount = 100,
                TotalReservations = 500,
                Latitude = 42.123,
                Longitude = 23.456,
                VenueType = new VenueTypeDto { Id = 1, Name = "Restaurant" }
            });

        _reviewsServiceMock
            .Setup(x => x.GetReviewsByVenueId(It.IsAny<int>()))
            .ReturnsAsync(new List<ReviewDto>());

        // Setup JSInterop
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Register services
        Services.AddSingleton(_venuesServiceMock.Object);
        Services.AddSingleton(_reviewsServiceMock.Object);
        Services.AddSingleton(_authHelperMock.Object);
        Services.AddSingleton(_configurationMock.Object);
    }

    #region Initialization Tests

    [Fact]
    public void VenueDetails_LoadsVenueData_OnInitialize()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        _venuesServiceMock.Verify(x => x.GetVenueById(1), Times.Once);
    }

    [Fact]
    public void VenueDetails_LoadsReviews_OnInitialize()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        _reviewsServiceMock.Verify(x => x.GetReviewsByVenueId(1), Times.Once);
    }

    [Fact]
    public void VenueDetails_LoadsUserId_OnInitialize()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        _authHelperMock.Verify(x => x.GetUserId(), Times.Once);
    }

    #endregion

    #region Venue Info Display Tests

    [Fact]
    public void VenueDetails_DisplaysVenueName()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto { Id = 1, Name = "My Awesome Restaurant" });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("My Awesome Restaurant", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysVenueDescription()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test",
                Description = "A wonderful dining experience"
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("A wonderful dining experience", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysNoDescriptionMessage_WhenEmpty()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test",
                Description = null
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("No description available", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysVenueRating()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test",
                Rating = 4.5,
                ReviewCount = 50
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("4.5", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysReviewCount()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test",
                ReviewCount = 125
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("125 reviews", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysTotalReservations()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test",
                TotalReservations = 500
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("500 total reservations", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysCoordinates()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test",
                Latitude = 42.123456,
                Longitude = 23.654321
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("42.123456", cut.Markup);
        Assert.Contains("23.654321", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysVenueType()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenueById(It.IsAny<int>()))
            .ReturnsAsync(new VenueSearchDto
            {
                Id = 1,
                Name = "Test",
                VenueType = new VenueTypeDto { Id = 1, Name = "Italian Restaurant" }
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Italian Restaurant", cut.Markup);
    }

    #endregion

    #region Section Headers Tests

    [Fact]
    public void VenueDetails_DisplaysDescriptionSection()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Description", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysAddressSection()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Address", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysCoordinatesLabel()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Coordinates", cut.Markup);
    }

    #endregion

    #region Reservation Button Tests

    [Fact]
    public void VenueDetails_DisplaysMakeReservationButton()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Make a Reservation", cut.Markup);
    }

    [Fact]
    public void VenueDetails_ReservationLink_ContainsVenueId()
    {
        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 5));

        // Assert
        Assert.Contains("/makeReservation/5", cut.Markup);
    }

    #endregion

    #region Review Form Tests

    [Fact]
    public void VenueDetails_DisplaysReviewForm_WhenUserLoggedIn()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync("user-123");

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Leave a Review", cut.Markup);
    }

    [Fact]
    public void VenueDetails_HidesReviewForm_WhenUserNotLoggedIn()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync((string?)null);

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.DoesNotContain("Leave a Review", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysRatingStars()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync("user-123");

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Rating", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysReviewTextarea()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync("user-123");

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Your Review", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysSubmitButton()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync("user-123");

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Submit", cut.Markup);
    }

    #endregion

    #region Reviews List Tests

    [Fact]
    public void VenueDetails_DisplaysLatestReviewsSection_WhenReviewsExist()
    {
        // Arrange
        _reviewsServiceMock
            .Setup(x => x.GetReviewsByVenueId(It.IsAny<int>()))
            .ReturnsAsync(new List<ReviewDto>
            {
                new ReviewDto
                {
                    Id = 1,
                    Rating = 5,
                    Comment = "Great place!",
                    ReviewerName = "John",
                    CreatedAt = DateTime.Now
                }
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Latest Reviews", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysReviewComment()
    {
        // Arrange
        _reviewsServiceMock
            .Setup(x => x.GetReviewsByVenueId(It.IsAny<int>()))
            .ReturnsAsync(new List<ReviewDto>
            {
                new ReviewDto
                {
                    Id = 1,
                    Comment = "Amazing food and service!",
                    CreatedAt = DateTime.Now
                }
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Amazing food and service!", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysReviewerName()
    {
        // Arrange
        _reviewsServiceMock
            .Setup(x => x.GetReviewsByVenueId(It.IsAny<int>()))
            .ReturnsAsync(new List<ReviewDto>
            {
                new ReviewDto
                {
                    Id = 1,
                    ReviewerName = "Maria Ivanova",
                    CreatedAt = DateTime.Now
                }
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Maria Ivanova", cut.Markup);
    }

    [Fact]
    public void VenueDetails_DisplaysAnonymous_WhenNoReviewerName()
    {
        // Arrange
        _reviewsServiceMock
            .Setup(x => x.GetReviewsByVenueId(It.IsAny<int>()))
            .ReturnsAsync(new List<ReviewDto>
            {
                new ReviewDto
                {
                    Id = 1,
                    ReviewerName = null,
                    CreatedAt = DateTime.Now
                }
            });

        // Act
        var cut = RenderComponent<VenueDetails>(parameters => parameters
            .Add(p => p.VenueId, 1));

        // Assert
        Assert.Contains("Anonymous", cut.Markup);
    }

    #endregion
}
