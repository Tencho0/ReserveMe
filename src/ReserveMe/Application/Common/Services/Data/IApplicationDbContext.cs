namespace Application.Common.Services.Data
{
	using Domain.Entities;
	using Microsoft.EntityFrameworkCore;

	public interface IApplicationDbContext
	{
		DbSet<TEntity> Set<TEntity>() where TEntity : class;

		DbSet<ApplicationUser> Users => Set<ApplicationUser>();

		DbSet<Venue> Venues => Set<Venue>();

		DbSet<VenueType> VenueTypes => Set<VenueType>();

		DbSet<Table> Tables => Set<Table>();

		DbSet<Reservation> Reservations => Set<Reservation>();

		DbSet<VenueFavorite> VenueFavorites => Set<VenueFavorite>();

		DbSet<VenueReview> VenueReviews => Set<VenueReview>();

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
