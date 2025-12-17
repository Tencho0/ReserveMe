namespace Infrastructure
{
	using System.Reflection;
	using Application.Common.Services.Data;
	using Domain.Entities;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		 : base(options)
		{
		}

		public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
		public DbSet<Venue> Venues => Set<Venue>();
		public DbSet<VenueType> VenueTypes => Set<VenueType>();
		public DbSet<Table> Tables => Set<Table>();
		public DbSet<Reservation> Reservations => Set<Reservation>();
		public DbSet<VenueFavorite> VenueFavorites => Set<VenueFavorite>();
		public DbSet<VenueReview> VenueReviews => Set<VenueReview>();

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
