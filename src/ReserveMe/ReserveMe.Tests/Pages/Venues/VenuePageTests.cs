namespace ReserveMe.Tests.Pages.Venues;

using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ReserveMe.Pages.Venues;
using Shared.Dtos.Venues;
using Shared.Dtos.VenueTypes;
using Shared.Services.Media;
using Shared.Services.Venues;
using Shared.Services.VenueTypes;
using System.Security.Claims;

/// <summary>
/// Unit tests for the VenuesPage (Admin) component.
/// Tests cover venue listing, creation, and deletion.
/// </summary>
public class VenuesPageTests : TestContext
{
    private readonly Mock<IVenuesService> _venuesServiceMock;
    private readonly Mock<IVenueTypesService> _venueTypesServiceMock;
    private readonly Mock<IMediaService> _mediaServiceMock;

    public VenuesPageTests()
    {
        _venuesServiceMock = new Mock<IVenuesService>();
        _venueTypesServiceMock = new Mock<IVenueTypesService>();
        _mediaServiceMock = new Mock<IMediaService>();

        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>());

        _venueTypesServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<VenueTypeDto>
            {
                new VenueTypeDto { Id = 1, Name = "Restaurant" },
                new VenueTypeDto { Id = 2, Name = "Bar" }
            });

        // Setup JSInterop
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Register services
        Services.AddSingleton(_venuesServiceMock.Object);
        Services.AddSingleton(_venueTypesServiceMock.Object);
        Services.AddSingleton(_mediaServiceMock.Object);

        // Use bUnit's TestAuthorizationContext for proper authorization support
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("admin@test.com");
        authContext.SetRoles("SuperAdmin");
    }

    #region Initialization Tests

    [Fact]
    public void VenuesPage_LoadsVenues_OnInitialize()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        _venuesServiceMock.Verify(x => x.GetVenues(), Times.Once);
    }

    [Fact]
    public void VenuesPage_LoadsVenueTypes_OnInitialize()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        _venueTypesServiceMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    #endregion

    #region Page Title Tests

    [Fact]
    public void VenuesPage_RendersPageTitle()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Manage Venues", cut.Markup);
    }

    #endregion

    #region Add Venue Button Tests

    [Fact]
    public void VenuesPage_RendersAddVenueButton()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Add new Venue", cut.Markup);
    }

    #endregion

    #region Table Headers Tests

    [Fact]
    public void VenuesPage_RendersTableHeaders()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Venue Name", cut.Markup);
        Assert.Contains("Description", cut.Markup);
        Assert.Contains("Venue Type", cut.Markup);
        Assert.Contains("Location", cut.Markup);
        Assert.Contains("Created At", cut.Markup);
        Assert.Contains("Actions", cut.Markup);
    }

    #endregion

    #region Venue List Tests

    [Fact]
    public void VenuesPage_DisplaysVenues_WhenDataExists()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto
                {
                    Id = 1,
                    Name = "Test Restaurant",
                    Description = "A great place",
                    Latitude = 42.0,
                    Longitude = 23.0,
                    CreatedAt = DateTime.Now,
                    VenueType = new VenueTypeDto { Id = 1, Name = "Restaurant" }
                }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Test Restaurant", cut.Markup);
    }

    [Fact]
    public void VenuesPage_DisplaysVenueDescription()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Amazing food here"
                }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Amazing food here", cut.Markup);
    }

    [Fact]
    public void VenuesPage_DisplaysVenueType()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto
                {
                    Id = 1,
                    Name = "Test",
                    VenueType = new VenueTypeDto { Id = 1, Name = "Italian Restaurant" }
                }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Italian Restaurant", cut.Markup);
    }

    [Fact]
    public void VenuesPage_DisplaysVenueLocation()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto
                {
                    Id = 1,
                    Name = "Test",
                    Latitude = 42.123,
                    Longitude = 23.456
                }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("23.456", cut.Markup);
        Assert.Contains("42.123", cut.Markup);
    }

    [Fact]
    public void VenuesPage_DisplaysCreatedAt()
    {
        // Arrange
        var createdDate = new DateTime(2024, 6, 15);
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto
                {
                    Id = 1,
                    Name = "Test",
                    CreatedAt = createdDate
                }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("15", cut.Markup);
        Assert.Contains("2024", cut.Markup);
    }

    [Fact]
    public void VenuesPage_DisplaysNoOwnerMessage_WhenNoVenueType()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto
                {
                    Id = 1,
                    Name = "Test",
                    VenueType = null
                }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("No Owner", cut.Markup);
    }

    #endregion

    #region Action Buttons Tests

    [Fact]
    public void VenuesPage_RendersEditButton()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto { Id = 1, Name = "Test" }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        var editButtons = cut.FindAll("a.btn-info-light");
        Assert.NotEmpty(editButtons);
    }

    [Fact]
    public void VenuesPage_RendersDeleteButton()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto { Id = 1, Name = "Test" }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        var deleteButtons = cut.FindAll("a.btn-danger-light");
        Assert.NotEmpty(deleteButtons);
    }

    [Fact]
    public void VenuesPage_EditLink_ContainsVenueId()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto { Id = 5, Name = "Test" }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("/admin/venueDetails/5", cut.Markup);
    }

    #endregion

    #region Create Venue Modal Tests

    [Fact]
    public void VenuesPage_CreateModal_HasVenueNameField()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Venue Name", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasDescriptionField()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert - Look for the label
        Assert.Contains("Description", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasVenueTypeDropdown()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Venue Type", cut.Markup);
        Assert.Contains("Select type", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasLatitudeField()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Latitude", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasLongitudeField()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Longitude", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasLogoUpload()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Logo", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasImageUpload()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Venue image", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasSaveButton()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Save", cut.Markup);
    }

    [Fact]
    public void VenuesPage_CreateModal_HasCancelButton()
    {
        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Cancel", cut.Markup);
    }

    #endregion

    #region Multiple Venues Tests

    [Fact]
    public void VenuesPage_DisplaysMultipleVenues()
    {
        // Arrange
        _venuesServiceMock
            .Setup(x => x.GetVenues())
            .ReturnsAsync(new List<VenueAdminDto>
            {
                new VenueAdminDto { Id = 1, Name = "Restaurant A" },
                new VenueAdminDto { Id = 2, Name = "Restaurant B" },
                new VenueAdminDto { Id = 3, Name = "Restaurant C" }
            });

        // Act
        var cut = RenderComponent<VenuesPage>();

        // Assert
        Assert.Contains("Restaurant A", cut.Markup);
        Assert.Contains("Restaurant B", cut.Markup);
        Assert.Contains("Restaurant C", cut.Markup);
    }

    #endregion
}
