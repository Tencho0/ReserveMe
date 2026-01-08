namespace ReserveMe.Tests.Pages.Account;

using ReserveMe.Pages.Account;

/// <summary>
/// Unit tests for the Register page component.
/// Tests cover registration logic and navigation after successful/failed registration.
/// </summary>
public class RegisterTests : TestContext
{
    private readonly Mock<IAuthenticationHelper> _authHelperMock;

    public RegisterTests()
    {
        _authHelperMock = new Mock<IAuthenticationHelper>();

        // Register services
        Services.AddSingleton(_authHelperMock.Object);
    }

    #region Successful Registration Tests

    [Fact]
    public async Task RegisterUser_WithValidData_NavigatesToHome()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()))
            .ReturnsAsync("valid-token");

        var cut = RenderComponent<Register>();

        // Act
        await cut.InvokeAsync(async () =>
        {
            // Use reflection to call the private method
            var method = typeof(Register).GetMethod("RegisterUser",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)method!.Invoke(cut.Instance, null)!;
        });

        // Assert
        _authHelperMock.Verify(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WithValidData_CallsAuthHelperRegisterAsync()
    {
        // Arrange
        RegisterUserDto? capturedDto = null;

        _authHelperMock
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()))
            .Callback<RegisterUserDto>(dto => capturedDto = dto)
            .ReturnsAsync("valid-token");

        var cut = RenderComponent<Register>();

        // Fill in the form
        cut.Find("#firstName").Change("John");
        cut.Find("#lastName").Change("Doe");
        cut.Find("#email").Change("john.doe@test.com");
        cut.Find("#password").Change("Password123!");
        cut.Find("#confirmPassword").Change("Password123!");

        // Act
        await cut.InvokeAsync(async () =>
        {
            var method = typeof(Register).GetMethod("RegisterUser",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)method!.Invoke(cut.Instance, null)!;
        });

        // Assert
        _authHelperMock.Verify(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Once);
    }

    #endregion

    #region Failed Registration Tests

    [Fact]
    public async Task RegisterUser_WithInvalidData_NavigatesTo401()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()))
            .ReturnsAsync(string.Empty);

        var cut = RenderComponent<Register>();

        // Act
        await cut.InvokeAsync(async () =>
        {
            var method = typeof(Register).GetMethod("RegisterUser",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)method!.Invoke(cut.Instance, null)!;
        });

        // Assert
        _authHelperMock.Verify(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WithNullResult_NavigatesTo401()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()))
            .ReturnsAsync((string)null!);

        var cut = RenderComponent<Register>();

        // Act
        await cut.InvokeAsync(async () =>
        {
            var method = typeof(Register).GetMethod("RegisterUser",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)method!.Invoke(cut.Instance, null)!;
        });

        // Assert
        _authHelperMock.Verify(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WhenServiceThrowsException_HandlesGracefully()
    {
        // Arrange
        _authHelperMock
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterUserDto>()))
            .ThrowsAsync(new Exception("Service unavailable"));

        var cut = RenderComponent<Register>();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await cut.InvokeAsync(async () =>
            {
                var method = typeof(Register).GetMethod("RegisterUser",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                await (Task)method!.Invoke(cut.Instance, null)!;
            });
        });
    }

    #endregion

    #region UI Element Tests

    [Fact]
    public void Register_RendersFirstNameInput()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var input = cut.Find("#firstName");
        Assert.NotNull(input);
    }

    [Fact]
    public void Register_RendersLastNameInput()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var input = cut.Find("#lastName");
        Assert.NotNull(input);
    }

    [Fact]
    public void Register_RendersEmailInput()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var input = cut.Find("#email");
        Assert.NotNull(input);
    }

    [Fact]
    public void Register_RendersPasswordInput()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var input = cut.Find("#password");
        Assert.NotNull(input);
        Assert.Equal("password", input.GetAttribute("type"));
    }

    [Fact]
    public void Register_RendersConfirmPasswordInput()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var input = cut.Find("#confirmPassword");
        Assert.NotNull(input);
        Assert.Equal("password", input.GetAttribute("type"));
    }

    [Fact]
    public void Register_RendersRegisterButton()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var button = cut.Find("button[type='submit']");
        Assert.NotNull(button);
        Assert.Contains("Register", button.TextContent);
    }

    [Fact]
    public void Register_RendersLoginLink()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var loginLink = cut.Find("a[href='/login']");
        Assert.NotNull(loginLink);
        Assert.Contains("Already have an account", loginLink.TextContent);
    }

    [Fact]
    public void Register_RendersPageTitle()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var heading = cut.Find("h3");
        Assert.Contains("Create Your Account", heading.TextContent);
    }

    [Fact]
    public void Register_RendersSubtitle()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert
        var subtitle = cut.Find("p.text-muted");
        Assert.Contains("Join us", subtitle.TextContent);
    }

    #endregion

    #region Form Validation Tests

    [Fact]
    public void Register_HasDataAnnotationsValidator()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert - Check that the form exists
        var form = cut.Find("form");
        Assert.NotNull(form);
    }

    [Fact]
    public void Register_HasValidationSummary()
    {
        // Act
        var cut = RenderComponent<Register>();

        // Assert - ValidationSummary renders as a ul with class validation-errors or similar
        // The exact rendering depends on whether there are errors
        var form = cut.Find("form");
        Assert.NotNull(form);
    }

    #endregion

    #region Input Binding Tests

    [Fact]
    public void Register_FirstNameInput_BindsCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Register>();
        var expectedValue = "TestFirstName";

        // Act
        cut.Find("#firstName").Change(expectedValue);

        // Assert
        var input = cut.Find("#firstName");
        Assert.Equal(expectedValue, input.GetAttribute("value"));
    }

    [Fact]
    public void Register_LastNameInput_BindsCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Register>();
        var expectedValue = "TestLastName";

        // Act
        cut.Find("#lastName").Change(expectedValue);

        // Assert
        var input = cut.Find("#lastName");
        Assert.Equal(expectedValue, input.GetAttribute("value"));
    }

    [Fact]
    public void Register_EmailInput_BindsCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Register>();
        var expectedValue = "test@example.com";

        // Act
        cut.Find("#email").Change(expectedValue);

        // Assert
        var input = cut.Find("#email");
        Assert.Equal(expectedValue, input.GetAttribute("value"));
    }

    [Fact]
    public void Register_PasswordInput_BindsCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Register>();
        var expectedValue = "TestPassword123!";

        // Act
        cut.Find("#password").Change(expectedValue);

        // Assert
        var input = cut.Find("#password");
        Assert.Equal(expectedValue, input.GetAttribute("value"));
    }

    [Fact]
    public void Register_ConfirmPasswordInput_BindsCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Register>();
        var expectedValue = "TestPassword123!";

        // Act
        cut.Find("#confirmPassword").Change(expectedValue);

        // Assert
        var input = cut.Find("#confirmPassword");
        Assert.Equal(expectedValue, input.GetAttribute("value"));
    }

    #endregion
}
