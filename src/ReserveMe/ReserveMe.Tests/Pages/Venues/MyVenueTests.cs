namespace ReserveMe.Tests.Pages.Venues;

using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using ReserveMe.Pages.Venues;
using Shared.Dtos.Users;
using Shared.Dtos.Venues;
using Shared.Helpers;
using Shared.Services.Users;
using Shared.Services.Venues;

/// <summary>
/// Unit tests for the MyVenue page component.
/// Tests cover venue loading, owner/waiter display, and delete modal interactions.
/// </summary>
public class MyVenueTests : TestContext
{
    private readonly Mock<IVenuesService> _venuesServiceMock;
    private readonly Mock<IAuthenticationHelper> _authHelperMock;
    private readonly Mock<IUserService> _userServiceMock;

    public MyVenueTests()
    {
        _venuesServiceMock = new Mock<IVenuesService>();
        _authHelperMock = new Mock<IAuthenticationHelper>();
        _userServiceMock = new Mock<IUserService>();

        // Default setup
        _authHelperMock
            .Setup(x => x.GetUserMenuId())
            .ReturnsAsync(1);

        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test Venue",
                Description = "Test Description",
                Latitude = 42.0,
                Longitude = 23.0,
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Register services
        Services.AddSingleton(_venuesServiceMock.Object);
        Services.AddSingleton(_authHelperMock.Object);
        Services.AddSingleton(_userServiceMock.Object);

        // Use bUnit's TestAuthorizationContext - this properly handles <AuthorizeView>
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("owner@test.com");
        authContext.SetRoles("Owner");
    }

    #region Initialization Tests

    [Fact]
    public void MyVenue_LoadsVenueData_OnInitialize()
    {
        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        _authHelperMock.Verify(x => x.GetUserMenuId(), Times.Once);
        _venuesServiceMock.Verify(x => x.GetMyVenue(1), Times.Once);
    }

    [Fact]
    public void MyVenue_DisplaysVenueName()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "My Restaurant",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("My Restaurant", cut.Markup);
    }

    [Fact]
    public void MyVenue_UsesAdminVenueId_WhenProvided()
    {
        // Act
        var cut = RenderComponent<MyVenue>(parameters => parameters
            .Add(p => p.AdminVenueId, 5));

        // Assert
        _venuesServiceMock.Verify(x => x.GetMyVenue(5), Times.Once);
    }

    #endregion

    #region Venue Info Display Tests

    [Fact]
    public void MyVenue_DisplaysVenueLocation()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Latitude = 42.123,
                Longitude = 23.456,
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("42.123", cut.Markup);
        Assert.Contains("23.456", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysVenueDescription()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Description = "A lovely restaurant",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("A lovely restaurant", cut.Markup);
    }

    #endregion

    #region Owners Section Tests

    [Fact]
    public void MyVenue_DisplaysOwnersSection()
    {
        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("Owners", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysNoOwnersMessage_WhenEmpty()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("No owners assigned", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysOwnersList_WhenOwnersExist()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Owners = new List<UserDto>
                {
                    new UserDto { Id = "1", FirstName = "John", LastName = "Doe", Email = "john@test.com", IsActive = true }
                },
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("John", cut.Markup);
        Assert.Contains("Doe", cut.Markup);
        Assert.Contains("john@test.com", cut.Markup);
    }

    #endregion

    #region Waiters Section Tests

    [Fact]
    public void MyVenue_DisplaysWaitersSection()
    {
        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("Venue Waiters", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysNoWaitersMessage_WhenEmpty()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("No Waiters Added", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysWaitersList_WhenWaitersExist()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>
                {
                    new UserDto { Id = "1", FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", IsActive = true }
                }
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("Jane", cut.Markup);
        Assert.Contains("Smith", cut.Markup);
        Assert.Contains("jane@test.com", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysActiveStatusBadge_ForActiveWaiter()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>
                {
                    new UserDto { Id = "1", FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", IsActive = true }
                }
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("Active", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysInactiveStatusBadge_ForInactiveWaiter()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>
                {
                    new UserDto { Id = "1", FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", IsActive = false }
                }
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("Inactive", cut.Markup);
    }

    #endregion

    #region Table Headers Tests

    [Fact]
    public void MyVenue_DisplaysOwnerTableHeaders()
    {
        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("Name", cut.Markup);
        Assert.Contains("Email/UserName", cut.Markup);
        Assert.Contains("PhoneNumber", cut.Markup);
        Assert.Contains("Status", cut.Markup);
    }

    #endregion

    #region Logo and Image Tests

    [Fact]
    public void MyVenue_DisplaysNoLogoMessage_WhenLogoUrlEmpty()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                LogoUrl = null,
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("No logo", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysNoImageMessage_WhenImageUrlEmpty()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                ImageUrl = null,
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("No image", cut.Markup);
    }

    [Fact]
    public void MyVenue_DisplaysLogoImage_WhenLogoUrlProvided()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                LogoUrl = "https://example.com/logo.png",
                Owners = new List<UserDto>(),
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert
        Assert.Contains("https://example.com/logo.png", cut.Markup);
    }

    #endregion

    #region SuperAdmin Actions Tests

    [Fact]
    public void MyVenue_ShowsActionsColumn_ForSuperAdmin()
    {
        // Arrange
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("admin@test.com");
        authContext.SetRoles("SuperAdmin");

        _venuesServiceMock
            .Setup(x => x.GetMyVenue(It.IsAny<int>()))
            .ReturnsAsync(new VenueDetailsDto
            {
                Id = 1,
                Name = "Test",
                Owners = new List<UserDto>
                {
                    new UserDto { Id = "1", FirstName = "Owner", LastName = "One", Email = "owner@test.com", IsActive = true }
                },
                Waiters = new List<UserDto>()
            });

        // Act
        var cut = RenderComponent<MyVenue>();

        // Assert - SuperAdmin should see Actions column for owners
        Assert.Contains("Actions", cut.Markup);
    }

    #endregion
}
