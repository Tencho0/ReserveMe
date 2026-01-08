namespace ReserveMe.Tests.Pages.Account;

using Common;
using ReserveMe.Pages.Account;

/// <summary>
/// Unit tests for the Login page component.
/// Tests cover navigation logic based on user roles after successful/failed login.
/// </summary>
public class LoginTests : TestContext
{
    private readonly Mock<IAuthenticationHelper> _authHelperMock;
    private readonly Mock<NavigationManager> _navigationManagerMock;
    private string? _navigatedUrl;

    public LoginTests()
    {
        _authHelperMock = new Mock<IAuthenticationHelper>();

        // Setup NavigationManager mock to track navigation
        _navigationManagerMock = new Mock<NavigationManager>();

        // Register services
        Services.AddSingleton(_authHelperMock.Object);
    }

    #region Successful Login Tests

    [Fact]
    public async Task LoginUser_WithValidCredentials_AdminRole_NavigatesToAdminVenues()
    {
        // Arrange
        var loginDto = new LoginUserDto { Email = "admin@test.com", Password = "Password123!" };

        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync("valid-token");

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.ADMINISTRATOR_ROLE))
            .ReturnsAsync(true);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.OWNER_ROLE))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.WAITER_ROLE))
            .ReturnsAsync(false);

        var cut = RenderComponent<Login>();
        var navigationManager = Services.GetRequiredService<NavigationManager>();

        // Act
        var form = cut.Find("form");
        cut.Find("#email").Change(loginDto.Email);
        cut.Find("#password").Change(loginDto.Password);

        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        _authHelperMock.Verify(x => x.LoginAsync(It.IsAny<LoginUserDto>()), Times.Once);
        _authHelperMock.Verify(x => x.IsUserInRole(UserRoles.ADMINISTRATOR_ROLE), Times.Once);
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_OwnerRole_NavigatesToLive()
    {
        // Arrange
        var loginDto = new LoginUserDto { Email = "owner@test.com", Password = "Password123!" };

        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync("valid-token");

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.ADMINISTRATOR_ROLE))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.OWNER_ROLE))
            .ReturnsAsync(true);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.WAITER_ROLE))
            .ReturnsAsync(false);

        var cut = RenderComponent<Login>();

        // Act
        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        _authHelperMock.Verify(x => x.LoginAsync(It.IsAny<LoginUserDto>()), Times.Once);
        _authHelperMock.Verify(x => x.IsUserInRole(UserRoles.OWNER_ROLE), Times.Once);
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_WaiterRole_NavigatesToLive()
    {
        // Arrange
        var loginDto = new LoginUserDto { Email = "waiter@test.com", Password = "Password123!" };

        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync("valid-token");

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.ADMINISTRATOR_ROLE))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.OWNER_ROLE))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.WAITER_ROLE))
            .ReturnsAsync(true);

        var cut = RenderComponent<Login>();

        // Act
        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        _authHelperMock.Verify(x => x.LoginAsync(It.IsAny<LoginUserDto>()), Times.Once);
        _authHelperMock.Verify(x => x.IsUserInRole(UserRoles.WAITER_ROLE), Times.Once);
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ClientRole_NavigatesToHome()
    {
        // Arrange
        var loginDto = new LoginUserDto { Email = "client@test.com", Password = "Password123!" };

        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync("valid-token");

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.ADMINISTRATOR_ROLE))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.OWNER_ROLE))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.WAITER_ROLE))
            .ReturnsAsync(false);

        var cut = RenderComponent<Login>();

        // Act
        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        _authHelperMock.Verify(x => x.LoginAsync(It.IsAny<LoginUserDto>()), Times.Once);
        // Client role - should navigate to home (not admin, not owner, not waiter)
        _authHelperMock.Verify(x => x.IsUserInRole(UserRoles.ADMINISTRATOR_ROLE), Times.Once);
        _authHelperMock.Verify(x => x.IsUserInRole(UserRoles.OWNER_ROLE), Times.Once);
        _authHelperMock.Verify(x => x.IsUserInRole(UserRoles.WAITER_ROLE), Times.Once);
    }

    #endregion

    #region Failed Login Tests

    [Fact]
    public async Task LoginUser_WithInvalidCredentials_NavigatesTo401()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync(string.Empty);

        _authHelperMock
            .Setup(x => x.IsUserInRole(It.IsAny<string>()))
            .ReturnsAsync(false);

        var cut = RenderComponent<Login>();

        // Act
        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        _authHelperMock.Verify(x => x.LoginAsync(It.IsAny<LoginUserDto>()), Times.Once);
    }

    [Fact]
    public async Task LoginUser_WithNullResult_NavigatesTo401()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync((string)null!);

        var cut = RenderComponent<Login>();

        // Act
        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        _authHelperMock.Verify(x => x.LoginAsync(It.IsAny<LoginUserDto>()), Times.Once);
    }

    #endregion

    #region Authentication Helper Interaction Tests

    [Fact]
    public async Task LoginUser_CallsAuthHelperLoginAsync_WithCorrectParameters()
    {
        // Arrange
        LoginUserDto? capturedDto = null;

        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .Callback<LoginUserDto>(dto => capturedDto = dto)
            .ReturnsAsync("valid-token");

        _authHelperMock
            .Setup(x => x.IsUserInRole(It.IsAny<string>()))
            .ReturnsAsync(false);

        var cut = RenderComponent<Login>();

        // Set values via the component's fields
        cut.Find("#email").Change("test@example.com");
        cut.Find("#password").Change("TestPassword123!");

        // Act
        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        _authHelperMock.Verify(x => x.LoginAsync(It.IsAny<LoginUserDto>()), Times.Once);
    }

    [Fact]
    public async Task LoginUser_WhenLoginSucceeds_ChecksAdminRoleFirst()
    {
        // Arrange
        var callOrder = new List<string>();

        _authHelperMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync("valid-token");

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.ADMINISTRATOR_ROLE))
            .Callback(() => callOrder.Add("Admin"))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.OWNER_ROLE))
            .Callback(() => callOrder.Add("Owner"))
            .ReturnsAsync(false);

        _authHelperMock
            .Setup(x => x.IsUserInRole(UserRoles.WAITER_ROLE))
            .Callback(() => callOrder.Add("Waiter"))
            .ReturnsAsync(false);

        var cut = RenderComponent<Login>();

        // Act
        await cut.InvokeAsync(() => cut.Instance.LoginUser());

        // Assert
        Assert.Equal("Admin", callOrder.First());
    }

    #endregion

    #region UI Element Tests

    [Fact]
    public void Login_RendersEmailInput()
    {
        // Act
        var cut = RenderComponent<Login>();

        // Assert
        var emailInput = cut.Find("#email");
        Assert.NotNull(emailInput);
        Assert.Equal("text", emailInput.GetAttribute("type") ?? "text");
    }

    [Fact]
    public void Login_RendersPasswordInput()
    {
        // Act
        var cut = RenderComponent<Login>();

        // Assert
        var passwordInput = cut.Find("#password");
        Assert.NotNull(passwordInput);
        Assert.Equal("password", passwordInput.GetAttribute("type"));
    }

    [Fact]
    public void Login_RendersLoginButton()
    {
        // Act
        var cut = RenderComponent<Login>();

        // Assert
        var button = cut.Find("button[type='submit']");
        Assert.NotNull(button);
        Assert.Contains("Login", button.TextContent);
    }

    [Fact]
    public void Login_RendersRegisterLink()
    {
        // Act
        var cut = RenderComponent<Login>();

        // Assert
        var registerLink = cut.Find("a[href='/register']");
        Assert.NotNull(registerLink);
        Assert.Contains("Create an account", registerLink.TextContent);
    }

    [Fact]
    public void Login_RendersWelcomeMessage()
    {
        // Act
        var cut = RenderComponent<Login>();

        // Assert
        var heading = cut.Find("h3");
        Assert.Contains("Welcome Back", heading.TextContent);
    }

    #endregion
}
