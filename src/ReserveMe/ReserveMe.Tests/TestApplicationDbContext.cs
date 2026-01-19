using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Domain.Entities;
using Table = Domain.Entities.Table;
using Application.Common.Services.Data;


public class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueType> VenueTypes => Set<VenueType>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<VenueFavorite> VenueFavorites => Set<VenueFavorite>();
    public DbSet<VenueReview> VenueReviews => Set<VenueReview>();

    public DatabaseFacade Database => base.Database;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => base.SaveChangesAsync(cancellationToken);
}
