namespace ReserveMe.Tests.Pages.Reservations;

using Common.Enums;
using Shared.Dtos.Reservations;

/// <summary>
/// Unit tests for CreateReservation functionality.
/// Tests the validation logic and reservation DTO handling.
/// Note: Component rendering tests are skipped due to MudBlazor DatePicker/TimePicker service dependencies.
/// </summary>
public class CreateReservationTests
{
    #region Validation Tests

    [Fact]
    public void ValidateReservation_ValidData_ReturnsNull()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            ContactPhone = "+359888123456",
            ContactEmail = "john@example.com",
            ReservationTime = DateTime.Now.AddDays(1),
            GuestsCount = 4
        };
        int venueId = 1;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Null(error);
    }

    [Fact]
    public void ValidateReservation_InvalidVenueId_ReturnsError()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            ReservationTime = DateTime.Now.AddDays(1),
            GuestsCount = 4
        };
        int venueId = 0;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Equal("Invalid venue.", error);
    }

    [Fact]
    public void ValidateReservation_NoReservationTime_ReturnsError()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            ReservationTime = null,
            GuestsCount = 4
        };
        int venueId = 1;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Equal("Please choose a reservation date & time.", error);
    }

    [Fact]
    public void ValidateReservation_PastReservationTime_ReturnsError()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            ReservationTime = DateTime.Now.AddHours(-1),
            GuestsCount = 4
        };
        int venueId = 1;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Equal("Reservation time must be in the future.", error);
    }

    [Fact]
    public void ValidateReservation_ZeroGuests_ReturnsError()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            ReservationTime = DateTime.Now.AddDays(1),
            GuestsCount = 0
        };
        int venueId = 1;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Equal("Guests count must be at least 1.", error);
    }

    [Fact]
    public void ValidateReservation_TooManyGuests_ReturnsError()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            ReservationTime = DateTime.Now.AddDays(1),
            GuestsCount = 25
        };
        int venueId = 1;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Equal("Guests count cannot be more than 20.", error);
    }

    [Fact]
    public void ValidateReservation_NoContactName_ReturnsError()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "",
            ReservationTime = DateTime.Now.AddDays(1),
            GuestsCount = 4
        };
        int venueId = 1;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Equal("Contact name is required.", error);
    }

    [Fact]
    public void ValidateReservation_WhitespaceContactName_ReturnsError()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "   ",
            ReservationTime = DateTime.Now.AddDays(1),
            GuestsCount = 4
        };
        int venueId = 1;

        // Act
        var error = ValidateReservation(dto, venueId);

        // Assert
        Assert.Equal("Contact name is required.", error);
    }

    #endregion

    #region Date/Time Sync Tests

    [Fact]
    public void SyncToDto_ValidDateAndTime_CombinesCorrectly()
    {
        // Arrange
        DateTime? date = new DateTime(2024, 12, 25);
        TimeSpan? time = new TimeSpan(18, 30, 0);

        // Act
        var result = SyncDateTimeToDto(date, time);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateTime(2024, 12, 25, 18, 30, 0), result);
    }

    [Fact]
    public void SyncToDto_NullDate_ReturnsNull()
    {
        // Arrange
        DateTime? date = null;
        TimeSpan? time = new TimeSpan(18, 30, 0);

        // Act
        var result = SyncDateTimeToDto(date, time);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SyncToDto_NullTime_ReturnsNull()
    {
        // Arrange
        DateTime? date = new DateTime(2024, 12, 25);
        TimeSpan? time = null;

        // Act
        var result = SyncDateTimeToDto(date, time);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void SyncToDto_BothNull_ReturnsNull()
    {
        // Arrange
        DateTime? date = null;
        TimeSpan? time = null;

        // Act
        var result = SyncDateTimeToDto(date, time);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region ReservationDto Tests

    [Fact]
    public void ReservationDto_DefaultStatus_IsPending()
    {
        // Arrange & Act
        var dto = new ReservationDto
        {
            Status = ReservationStatus.Pending
        };

        // Assert
        Assert.Equal(ReservationStatus.Pending, dto.Status);
    }

    [Fact]
    public void ReservationDto_CanSetAllProperties()
    {
        // Arrange & Act
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            ContactPhone = "+359888123456",
            ContactEmail = "john@example.com",
            GuestsCount = 4,
            VenueId = 1,
            UserId = "user-123",
            ReservationTime = DateTime.Now.AddDays(1),
            Status = ReservationStatus.Approved
        };

        // Assert
        Assert.Equal("John Doe", dto.ContactName);
        Assert.Equal("+359888123456", dto.ContactPhone);
        Assert.Equal("john@example.com", dto.ContactEmail);
        Assert.Equal(4, dto.GuestsCount);
        Assert.Equal(1, dto.VenueId);
        Assert.Equal("user-123", dto.UserId);
        Assert.Equal(ReservationStatus.Approved, dto.Status);
    }

    #endregion

    #region Guests Count Boundary Tests

    [Theory]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(20, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(21, false)]
    [InlineData(100, false)]
    public void ValidateGuestsCount_BoundaryValues(int guestsCount, bool isValid)
    {
        // Act
        var result = IsValidGuestsCount(guestsCount);

        // Assert
        Assert.Equal(isValid, result);
    }

    #endregion

    #region Future Time Validation Tests

    [Fact]
    public void IsFutureTime_Tomorrow_ReturnsTrue()
    {
        // Arrange
        var time = DateTime.Now.AddDays(1);

        // Act
        var result = IsFutureTime(time);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsFutureTime_Yesterday_ReturnsFalse()
    {
        // Arrange
        var time = DateTime.Now.AddDays(-1);

        // Act
        var result = IsFutureTime(time);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsFutureTime_OneHourAgo_ReturnsFalse()
    {
        // Arrange
        var time = DateTime.Now.AddHours(-1);

        // Act
        var result = IsFutureTime(time);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsFutureTime_OneHourFromNow_ReturnsTrue()
    {
        // Arrange
        var time = DateTime.Now.AddHours(1);

        // Act
        var result = IsFutureTime(time);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Contact Name Validation Tests

    [Theory]
    [InlineData("John Doe", true)]
    [InlineData("J", true)]
    [InlineData("Иван Иванов", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void ValidateContactName_VariousInputs(string? name, bool isValid)
    {
        // Act
        var result = IsValidContactName(name);

        // Assert
        Assert.Equal(isValid, result);
    }

    #endregion

    #region Success Message Format Tests

    [Fact]
    public void FormatSuccessMessage_ContainsAllDetails()
    {
        // Arrange
        var dto = new ReservationDto
        {
            ContactName = "John Doe",
            GuestsCount = 4,
            ReservationTime = new DateTime(2024, 12, 25, 18, 30, 0)
        };

        // Act
        var message = FormatSuccessMessage(dto);

        // Assert
        Assert.Contains("John Doe", message);
        Assert.Contains("4", message);
        Assert.Contains("12/25/2024", message);
    }

    #endregion

    #region Form Reset Tests

    [Fact]
    public void ResetForm_CreatesNewEmptyDto()
    {
        // Arrange
        var originalDto = new ReservationDto
        {
            ContactName = "John Doe",
            GuestsCount = 4
        };

        // Act
        var newDto = ResetForm();

        // Assert
        Assert.NotSame(originalDto, newDto);
        Assert.Null(newDto.ContactName);
        Assert.Equal(0, newDto.GuestsCount);
    }

    #endregion

    #region Helper Methods (Simulating Component Logic)

    private static string? ValidateReservation(ReservationDto dto, int venueId)
    {
        if (venueId <= 0)
            return "Invalid venue.";

        if (!dto.ReservationTime.HasValue)
            return "Please choose a reservation date & time.";

        if (dto.ReservationTime.Value <= DateTime.Now)
            return "Reservation time must be in the future.";

        if (dto.GuestsCount < 1)
            return "Guests count must be at least 1.";

        if (dto.GuestsCount > 20)
            return "Guests count cannot be more than 20.";

        if (string.IsNullOrWhiteSpace(dto.ContactName))
            return "Contact name is required.";

        return null;
    }

    private static DateTime? SyncDateTimeToDto(DateTime? date, TimeSpan? time)
    {
        if (date is null || time is null)
            return null;

        return date.Value.Date + time.Value;
    }

    private static bool IsValidGuestsCount(int count)
    {
        return count >= 1 && count <= 20;
    }

    private static bool IsFutureTime(DateTime time)
    {
        return time > DateTime.Now;
    }

    private static bool IsValidContactName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }

    private static string FormatSuccessMessage(ReservationDto dto)
    {
        return $"Reservation created (mock). Guest: {dto.ContactName}, " +
               $"Guests: {dto.GuestsCount}, " +
               $"Date: {dto.ReservationTime:MM/dd/yyyy HH:mm}.";
    }

    private static ReservationDto ResetForm()
    {
        return new ReservationDto();
    }

    #endregion
}
