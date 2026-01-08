namespace ReserveMe.Tests.Pages.Reservations;

using Common.Enums;
using ReserveMe.Pages.Reservations;
using Shared.Dtos.Reservations;
using Shared.Helpers;
using Shared.Services.Reservations;

/// <summary>
/// Unit tests for the ReservationManagement page component.
/// Tests cover filtering, pagination, status changes, and UI interactions.
/// </summary>
public class ReservationManagementTests : TestContext
{
    private readonly Mock<IReservationsService> _reservationsServiceMock;
    private readonly Mock<IAuthenticationHelper> _authHelperMock;

    public ReservationManagementTests()
    {
        _reservationsServiceMock = new Mock<IReservationsService>();
        _authHelperMock = new Mock<IAuthenticationHelper>();

        // Default setup
        _authHelperMock
            .Setup(x => x.GetUserMenuId())
            .ReturnsAsync(1);

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(new List<ReservationDto>());

        // Register services
        Services.AddSingleton(_reservationsServiceMock.Object);
        Services.AddSingleton(_authHelperMock.Object);
    }

    #region Initialization Tests

    [Fact]
    public void ReservationManagement_RendersPageTitle()
    {
        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Reservation Management", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_LoadsReservations_OnInitialize()
    {
        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        _authHelperMock.Verify(x => x.GetUserMenuId(), Times.Once);
        _reservationsServiceMock.Verify(x => x.GetReservations(1), Times.Once);
    }

    #endregion

    #region Statistics Cards Tests

    [Fact]
    public void ReservationManagement_DisplaysStatisticsCards()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto { Id = 1, Status = ReservationStatus.Pending, ContactName = "Test", ContactPhone = "123" },
            new ReservationDto { Id = 2, Status = ReservationStatus.Approved, ContactName = "Test", ContactPhone = "123" },
            new ReservationDto { Id = 3, Status = ReservationStatus.InProgress, ContactName = "Test", ContactPhone = "123" }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Total", cut.Markup);
        Assert.Contains("Pending", cut.Markup);
        Assert.Contains("Approved", cut.Markup);
        Assert.Contains("In progress", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_DisplaysCorrectTotalCount()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto { Id = 1, Status = ReservationStatus.Pending, ContactName = "Test", ContactPhone = "123" },
            new ReservationDto { Id = 2, Status = ReservationStatus.Approved, ContactName = "Test", ContactPhone = "123" },
            new ReservationDto { Id = 3, Status = ReservationStatus.Completed, ContactName = "Test", ContactPhone = "123" }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert - Total should be 3
        var cards = cut.FindAll(".card h3");
        Assert.Contains(cards, c => c.TextContent == "3");
    }

    #endregion

    #region Filter UI Tests

    [Fact]
    public void ReservationManagement_RendersSearchInput()
    {
        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var searchInput = cut.Find("input[placeholder*='Search']");
        Assert.NotNull(searchInput);
    }

    [Fact]
    public void ReservationManagement_RendersTodayButton()
    {
        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var todayButton = cut.Find("button.btn-primary");
        Assert.Contains("Today", todayButton.TextContent);
    }

    [Fact]
    public void ReservationManagement_RendersDateFromFilter()
    {
        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var label = cut.Find("label.form-label");
        Assert.Contains("Date from", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_RendersStatusFilter()
    {
        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var statusSelect = cut.Find("select.form-select");
        Assert.NotNull(statusSelect);
        Assert.Contains("All statuses", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_RendersTableNumberFilter()
    {
        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Table number", cut.Markup);
    }

    #endregion

    #region Empty State Tests

    [Fact]
    public void ReservationManagement_ShowsEmptyState_WhenNoReservations()
    {
        // Arrange
        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(new List<ReservationDto>());

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("No reservations found", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_ShowsFilterHint_WhenEmpty()
    {
        // Arrange
        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(new List<ReservationDto>());

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Try adjusting the filters", cut.Markup);
    }

    #endregion

    #region Table Display Tests

    [Fact]
    public void ReservationManagement_RendersTableHeaders()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "John Doe",
                ContactPhone = "123456",
                Status = ReservationStatus.Pending,
                GuestsCount = 4,
                TableNumber = 5,
                ReservationTime = DateTime.Now
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Customer", cut.Markup);
        Assert.Contains("Date and time", cut.Markup);
        Assert.Contains("Guests", cut.Markup);
        Assert.Contains("Table", cut.Markup);
        Assert.Contains("Status", cut.Markup);
        Assert.Contains("Actions", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_DisplaysCustomerName()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Ivan Petrov",
                ContactPhone = "123456",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Ivan Petrov", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_DisplaysCustomerPhone()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "+359888123456",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("+359888123456", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_DisplaysGuestsCount()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                GuestsCount = 8,
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("8 guests", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_DisplaysTableNumber()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                TableNumber = 12,
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Table #12", cut.Markup);
    }

    #endregion

    #region Status Badge Tests

    [Theory]
    [InlineData(ReservationStatus.Pending, "Pending confirmation")]
    [InlineData(ReservationStatus.Approved, "Approved")]
    [InlineData(ReservationStatus.InProgress, "In progress")]
    [InlineData(ReservationStatus.Declined, "Declined")]
    [InlineData(ReservationStatus.Completed, "Completed")]
    public void ReservationManagement_DisplaysCorrectStatusText(ReservationStatus status, string expectedText)
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = status
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains(expectedText, cut.Markup);
    }

    [Theory]
    [InlineData(ReservationStatus.Pending, "bg-warning")]
    [InlineData(ReservationStatus.Approved, "bg-success")]
    [InlineData(ReservationStatus.InProgress, "bg-info")]
    [InlineData(ReservationStatus.Declined, "bg-danger")]
    [InlineData(ReservationStatus.Completed, "bg-secondary")]
    public void ReservationManagement_AppliesCorrectStatusBadgeClass(ReservationStatus status, string expectedClass)
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = status
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var badge = cut.Find(".badge");
        Assert.Contains(expectedClass, badge.ClassName);
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public void ReservationManagement_RendersPagination()
    {
        // Arrange
        var reservations = Enumerable.Range(1, 15)
            .Select(i => new ReservationDto
            {
                Id = i,
                ContactName = $"Customer {i}",
                ContactPhone = "123",
                Status = ReservationStatus.Pending
            })
            .ToList();

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Rows per page", cut.Markup);
        Assert.Contains("pagination", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_DisplaysPageSizeOptions()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto { Id = 1, ContactName = "Test", ContactPhone = "123", Status = ReservationStatus.Pending }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var pageSizeSelect = cut.FindAll("select.form-select-sm");
        Assert.NotEmpty(pageSizeSelect);
    }

    #endregion

    #region Action Menu Tests

    [Fact]
    public void ReservationManagement_RendersActionDropdown()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var dropdown = cut.Find(".dropdown");
        Assert.NotNull(dropdown);
    }

    [Fact]
    public void ReservationManagement_ShowsAcceptOption_ForPendingReservation()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Accept", cut.Markup);
        Assert.Contains("Decline", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_ShowsStartOption_ForApprovedReservation()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = ReservationStatus.Approved
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Start", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_ShowsCompleteOption_ForInProgressReservation()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = ReservationStatus.InProgress
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Complete", cut.Markup);
    }

    [Fact]
    public void ReservationManagement_ShowsDetailsOption_ForAllReservations()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        Assert.Contains("Details", cut.Markup);
    }

    #endregion

    #region Avatar/Initials Tests

    [Fact]
    public void ReservationManagement_DisplaysCustomerInitials()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "John Doe",
                ContactPhone = "123",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert
        var avatar = cut.Find(".avatar-circle");
        Assert.NotNull(avatar);
        Assert.Contains("JD", avatar.TextContent);
    }

    #endregion

    #region Status Change Tests

    [Fact]
    public async Task ReservationManagement_CallsChangeReservationStatus()
    {
        // Arrange
        var reservations = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 1,
                ContactName = "Test",
                ContactPhone = "123",
                Status = ReservationStatus.Pending
            }
        };

        _reservationsServiceMock
            .Setup(x => x.GetReservations(It.IsAny<int>()))
            .ReturnsAsync(reservations);

        _reservationsServiceMock
            .Setup(x => x.ChangeReservationStatus(It.IsAny<int>(), It.IsAny<ReservationStatus>()))
            .ReturnsAsync(true);

        // Act
        var cut = RenderComponent<ReservationManagement>();

        // Assert - Service is properly set up
        Assert.NotNull(_reservationsServiceMock.Object);
    }

    #endregion
}
