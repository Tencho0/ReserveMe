using System;
using System.Threading.Tasks;
using Application.Reservations.Commands;
using Common.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using Shared.Requests.Reservations;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;

public class CreateReservationCommandHandlerTests
{
    private UserManager<ApplicationUser> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new UserManager<ApplicationUser>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static TestApplicationDbContext CreateDb()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new TestApplicationDbContext(options);
        db.Database.EnsureCreated(); // important for sqlite in-memory
        return db;
    }


    [Fact]
    public async Task Creates_reservation_when_capacity_available_in_overlap_window()
    {
        await using var db = CreateDb();

        db.Venues.Add(new Venue { Id = 2, IsActive = true, IsDeleted = false, Name = "V2" });
        db.Tables.AddRange(
            new Table { VenueId = 2, TableNumber = 1, Capacity = 4, IsActive = true },
            new Table { VenueId = 2, TableNumber = 2, Capacity = 4, IsActive = true }
        ); // total capacity = 8

        db.Reservations.Add(new Reservation
        {
            VenueId = 2,
            GuestsCount = 3,
            ReservationTime = new DateTime(2026, 1, 18, 18, 0, 0),
            Status = ReservationStatus.Approved
        });

        await db.SaveChangesAsync(default);

        var handler = new CreateReservationCommandHandler(db, CreateUserManagerMock());

        var req = new SaveReservationRequest
        {
            UserId = null,
            VenueId = 2,
            TableNumber = 1,
            GuestsCount = 4,
            ReservationTime = new DateTime(2026, 1, 18, 19, 0, 0),
            Status = (int)ReservationStatus.Pending
        };

        await handler.Handle(new CreateReservationCommand(req), default);

        var count = await db.Reservations.CountAsync(r => r.VenueId == 2);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Throws_capacity_exceeded_when_overlap_total_exceeds_venue_capacity()
    {
        await using var db = CreateDb();

        db.Venues.Add(new Venue { Id = 2, IsActive = true, IsDeleted = false, Name = "V2" });
        db.Tables.Add(new Table { VenueId = 2, TableNumber = 1, Capacity = 6, IsActive = true }); // capacity = 6

        db.Reservations.Add(new Reservation
        {
            VenueId = 2,
            GuestsCount = 4,
            ReservationTime = new DateTime(2026, 1, 18, 18, 0, 0),
            Status = ReservationStatus.InProgress
        });

        await db.SaveChangesAsync(default);

        var handler = new CreateReservationCommandHandler(db, CreateUserManagerMock());

        var req = new SaveReservationRequest
        {
            UserId = null,
            VenueId = 2,
            TableNumber = 1,
            GuestsCount = 3, // 4 + 3 > 6 => should fail
            ReservationTime = new DateTime(2026, 1, 18, 19, 0, 0),
            Status = (int)ReservationStatus.Pending
        };

        await Assert.ThrowsAsync<ReservationCapacityExceededException>(
            () => handler.Handle(new CreateReservationCommand(req), default)
        );
    }

    [Fact]
    public async Task Does_not_count_declined_or_completed_reservations_towards_capacity()
    {
        await using var db = CreateDb();

        db.Venues.Add(new Venue { Id = 2, IsActive = true, IsDeleted = false, Name = "V2" });
        db.Tables.Add(new Table { VenueId = 2, TableNumber = 1, Capacity = 5, IsActive = true }); // capacity = 5

        db.Reservations.AddRange(
            new Reservation
            {
                VenueId = 2,
                GuestsCount = 5,
                ReservationTime = new DateTime(2026, 1, 18, 18, 0, 0),
                Status = ReservationStatus.Declined
            },
            new Reservation
            {
                VenueId = 2,
                GuestsCount = 5,
                ReservationTime = new DateTime(2026, 1, 18, 18, 30, 0),
                Status = ReservationStatus.Completed
            });

        await db.SaveChangesAsync(default);

        var handler = new CreateReservationCommandHandler(db, CreateUserManagerMock());

        var req = new SaveReservationRequest
        {
            UserId = null,
            VenueId = 2,
            TableNumber = 1,
            GuestsCount = 5,
            ReservationTime = new DateTime(2026, 1, 18, 19, 0, 0),
            Status = (int)ReservationStatus.Pending
        };

        await handler.Handle(new CreateReservationCommand(req), default);

        var count = await db.Reservations.CountAsync(r => r.VenueId == 2);
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task Fills_contact_data_from_user_profile_if_not_provided()
    {
        await using var db = CreateDb();

        var userId = "user-123";
        var user = new ApplicationUser
        {
            Id = userId,
            FirstName = "Ivan",
            LastName = "Ivanov",
            Email = "ivan@test.com",
            PhoneNumber = "123456"
        };

        var store = new Mock<IUserStore<ApplicationUser>>();
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);

        db.Venues.Add(new Venue { Id = 3, IsActive = true, IsDeleted = false, Name = "V3" });
        db.Tables.Add(new Table { VenueId = 3, TableNumber = 1, Capacity = 10, IsActive = true });
        await db.SaveChangesAsync();

        var handler = new CreateReservationCommandHandler(db, userManagerMock.Object);

        var req = new SaveReservationRequest
        {
            UserId = userId,
            VenueId = 3,
            TableNumber = 1,
            GuestsCount = 2,
            ReservationTime = DateTime.Now.AddDays(1),
            Status = (int)ReservationStatus.Pending
            // Contact fields are null
        };

        await handler.Handle(new CreateReservationCommand(req), default);

        var reservation = await db.Reservations.FirstAsync(r => r.UserId == userId);
        Assert.Equal("Ivan Ivanov", reservation.ContactName);
        Assert.Equal("ivan@test.com", reservation.ContactEmail);
        Assert.Equal("123456", reservation.ContactPhone);
    }
}
