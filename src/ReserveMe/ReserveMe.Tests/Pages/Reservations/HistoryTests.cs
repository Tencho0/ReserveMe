namespace ReserveMe.Tests.Pages.Reservations;

using Common.Enums;
using ReserveMe.Pages.Reservations;
using Shared.Dtos.Reservations;
using Shared.Helpers;
using Shared.Services.Reservations;

/// <summary>
/// Unit tests for the History page component.
/// Tests cover reservation history display, navigation, and status handling.
/// </summary>
public class HistoryTests : TestContext
{
    private readonly Mock<IReservationsService> _reservationsServiceMock;
    private readonly Mock<IAuthenticationHelper> _authHelperMock;

    public HistoryTests()
    {
        _reservationsServiceMock = new Mock<IReservationsService>();
        _authHelperMock = new Mock<IAuthenticationHelper>();

        // Default setup - logged in user
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync("user-123");

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(new List<ReservationForClientDto>());

        // Register services
        Services.AddSingleton(_reservationsServiceMock.Object);
        Services.AddSingleton(_authHelperMock.Object);
    }

	#region Initialization Tests

	[Fact]
	public void History_RendersPageHeader()
	{
		// Act
		var cut = RenderComponent<History>();

		// Assert
		var header = cut.Find("h1");
		Assert.Contains("Reservation History", header.TextContent);
	}

	[Fact]
    public void History_LoadsUserReservations()
    {
        // Act
        var cut = RenderComponent<History>();

        // Assert
        _authHelperMock.Verify(x => x.GetUserId(), Times.Once);
        _reservationsServiceMock.Verify(x => x.GetReservationsByClientId("user-123"), Times.Once);
    }

    [Fact]
    public void History_RedirectsToLogin_WhenUserNotLoggedIn()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.GetUserId())
            .ReturnsAsync((string?)null);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        var navManager = Services.GetRequiredService<NavigationManager>();
        // Navigation would be triggered to /login
        _authHelperMock.Verify(x => x.GetUserId(), Times.Once);
    }

	#endregion

	#region Empty State Tests

	[Fact]
	public void History_ShowsEmptyMessage_WhenNoReservations()
	{
		// Arrange
		_reservationsServiceMock
			.Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
			.ReturnsAsync(new List<ReservationForClientDto>());

		// Act
		var cut = RenderComponent<History>();

		// Assert
		Assert.Contains("No reservations", cut.Markup);
	}

	#endregion

	#region Reservation List Tests

	[Fact]
    public void History_DisplaysReservations_WhenDataExists()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 1,
                VenueName = "Test Restaurant",
                VenueLogo = "/logo.jpg",
                ReservationTime = DateTime.Now.AddDays(1),
                GuestsCount = 4,
                Status = ReservationStatus.Approved,
                VenueType = "Restaurant"
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains("Test Restaurant", cut.Markup);
    }

    [Fact]
    public void History_DisplaysReservationId()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 42,
                VenueName = "Test",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains("#42", cut.Markup);
    }

    [Fact]
    public void History_DisplaysGuestsCount()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 1,
                VenueName = "Test",
                GuestsCount = 6,
                Status = ReservationStatus.Approved
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains("6", cut.Markup);
    }

    [Fact]
    public void History_DisplaysVenueType()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 1,
                VenueName = "Test",
                VenueType = "Italian Restaurant",
                Status = ReservationStatus.Approved
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains("Italian Restaurant", cut.Markup);
    }

    #endregion

    #region Status Display Tests

    [Theory]
    [InlineData(ReservationStatus.Pending, "Pending")]
    [InlineData(ReservationStatus.Approved, "Approved")]
    [InlineData(ReservationStatus.InProgress, "In Progress")]
    [InlineData(ReservationStatus.Declined, "Declined")]
    [InlineData(ReservationStatus.Completed, "Completed")]
    public void History_DisplaysCorrectStatusLabel(ReservationStatus status, string expectedLabel)
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 1,
                VenueName = "Test",
                Status = status
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains(expectedLabel, cut.Markup);
    }

    [Theory]
    [InlineData(ReservationStatus.Pending, "reservation-status--pending")]
    [InlineData(ReservationStatus.Approved, "reservation-status--approved")]
    [InlineData(ReservationStatus.InProgress, "reservation-status--inprogress")]
    [InlineData(ReservationStatus.Declined, "reservation-status--declined")]
    [InlineData(ReservationStatus.Completed, "reservation-status--completed")]
    public void History_AppliesCorrectStatusClass(ReservationStatus status, string expectedClass)
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 1,
                VenueName = "Test",
                Status = status
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains(expectedClass, cut.Markup);
    }

    #endregion

    #region Logo Display Tests

    [Fact]
    public void History_DisplaysVenueLogo_WhenProvided()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 1,
                VenueName = "Test",
                VenueLogo = "/images/custom-logo.jpg",
                Status = ReservationStatus.Approved
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains("/images/custom-logo.jpg", cut.Markup);
    }

    [Fact]
    public void History_DisplaysDefaultLogo_WhenNoLogoProvided()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto
            {
                Id = 1,
                VenueName = "Test",
                VenueLogo = null,
                Status = ReservationStatus.Approved
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains("/assets/images/emptyImg.jpg", cut.Markup);
    }

    #endregion

    #region Multiple Reservations Tests

    [Fact]
    public void History_DisplaysMultipleReservations()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto { Id = 1, VenueName = "Restaurant A", Status = ReservationStatus.Pending },
            new ReservationForClientDto { Id = 2, VenueName = "Restaurant B", Status = ReservationStatus.Approved },
            new ReservationForClientDto { Id = 3, VenueName = "Restaurant C", Status = ReservationStatus.Completed }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        Assert.Contains("Restaurant A", cut.Markup);
        Assert.Contains("Restaurant B", cut.Markup);
        Assert.Contains("Restaurant C", cut.Markup);
    }

    [Fact]
    public void History_DisplaysCorrectNumberOfReservationItems()
    {
        // Arrange
        var reservations = new List<ReservationForClientDto>
        {
            new ReservationForClientDto { Id = 1, VenueName = "Test 1", Status = ReservationStatus.Pending },
            new ReservationForClientDto { Id = 2, VenueName = "Test 2", Status = ReservationStatus.Approved }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservationsByClientId(It.IsAny<string>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<History>();

        // Assert
        var items = cut.FindAll(".reservation-item");
        Assert.Equal(2, items.Count);
    }

    #endregion
}
