namespace Infrastructure.DataSeeder
{
	using Application.Helpers;
	using Common;
	using Domain.Entities;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;

	public class AppDataSeeder
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		string _defaultPassword = "ReserveMe2@25";

		string _envPath = string.Empty;
		string _storagePath = "Storage";

		public AppDataSeeder(
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			string envPath)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
			_envPath = envPath;
		}

		public async Task SeedAllAsync(CancellationToken cancellationToken)
		{
			if (_context.Roles.Any())
			{
				return;
			}

			// 1. Seed App User Roles
			var roles = await UsersRolesDataSeeder.SeedData(_context, _roleManager, cancellationToken);
			await SeedAdministratorsAsync(cancellationToken);
			await SeedAppUsersAsync(cancellationToken);

			// 3. Seed Venues with VenueTypes
			var venues = await SeedVenuesAsync(cancellationToken);
		}

		private async Task<List<Venue>> SeedVenuesAsync(CancellationToken cancellationToken)
		{
			var items = new List<Venue>();

			try
			{
				// 1) Ensure VenueTypes are seeded
				if (!await _context.VenueTypes.AnyAsync(cancellationToken))
				{
					var venueTypes = new List<VenueType>
					{
						new VenueType { Name = "Restaurant" },
						new VenueType { Name = "Bar" },
						new VenueType { Name = "Cafe" },
						new VenueType { Name = "Pub" },
						new VenueType { Name = "Bistro" },
					};

					_context.VenueTypes.AddRange(venueTypes);
					await _context.SaveChangesAsync(cancellationToken);
				}

				// 2) Load all VenueTypes from DB
				var existingVenueTypes = await _context.VenueTypes
					.AsNoTracking()
					.ToListAsync(cancellationToken);

				if (!existingVenueTypes.Any())
				{
					return items;
				}

				// 3) Seed Venues and assign a VenueType to each
				for (var i = 0; i < 5; i++)
				{
					// e.g. rotate through types: 0,1,2,3,4,0,1,...
					var type = existingVenueTypes[i % existingVenueTypes.Count];

					items.Add(new Venue
					{
						VenueTypeId = type.Id,
						Name = $"Venue {i + 1}",
						Description = $"Description {i + 1}",
						Latitude = 42.3915,
						Longitude = 23.2111,
						IsActive = true,
						IsDeleted = false,
						LogoUrl = string.Empty,
						CreatedAt = DateTime.Now
					});
				}

				_context.Venues.AddRange(items);
				await _context.SaveChangesAsync(cancellationToken);

				var profileStoragePath = Path.Combine(_envPath, _storagePath);

				#region Users in venues

				// Add Owner and Waiters per menu
				for (var i = 0; i < items.Count; i++)
				{
					// Add Owner
					var owner = new ApplicationUser()
					{
						VenueId = items[i].Id,
						FirstName = "Owner",
						LastName = items[i].Name,
						UserName = $"owner{i.ToString("D2")}@local.com",
						Email = $"owner{i.ToString("D2")}@local.com",
						EmailConfirmed = true
					};
					owner.ProfilePicture = Task.Run(() => RandomFaceGenerator.GetRandomFaceAsync(profileStoragePath)).Result;
					var result = await _userManager.CreateAsync(owner, _defaultPassword);
					if (result.Succeeded)
					{
						await _userManager.AddToRoleAsync(owner, UserRoles.OWNER_ROLE);
					}

					// Add 3 waiters
					for (int y = 0; y < 3; y++)
					{
						var waiter = new ApplicationUser()
						{
							VenueId = items[i].Id,
							FirstName = "Waiter",
							LastName = $"User {y.ToString("D2")}",
							UserName = $"waiter{i.ToString("D2")}{y.ToString("D2")}@local.com",
							Email = $"waiter{i.ToString("D2")}{y.ToString("D2")}@local.com",
							EmailConfirmed = true
						};
						waiter.ProfilePicture = Task.Run(() => RandomFaceGenerator.GetRandomFaceAsync(profileStoragePath)).Result;
						var waiterResult = await _userManager.CreateAsync(waiter, _defaultPassword);
						if (waiterResult.Succeeded)
						{
							await _userManager.AddToRoleAsync(waiter, UserRoles.WAITER_ROLE);
						}
					}
				}

				#endregion
			}
			catch (Exception ex)
			{
			}

			return items;
		}

		private async Task SeedAdministratorsAsync(CancellationToken cancellationToken)
		{
			try
			{
				var profileStoragePath = Path.Combine(_envPath, _storagePath);

				// Default Admin
				var defaultAdmin = new ApplicationUser()
				{
					FirstName = "Admin",
					LastName = $"User",
					UserName = $"superadmin@reserveme.com",
					Email = $"superadmin@reserveme.com",
					EmailConfirmed = true
				};
				defaultAdmin.ProfilePicture = Task.Run(() => RandomFaceGenerator.GetRandomFaceAsync(profileStoragePath)).Result;
				var defaultResult = await _userManager.CreateAsync(defaultAdmin, _defaultPassword);
				if (defaultResult.Succeeded)
				{
					await _userManager.AddToRoleAsync(defaultAdmin, UserRoles.ADMINISTRATOR_ROLE);
				}
			}
			catch (Exception ex)
			{ }
		}

		private async Task SeedAppUsersAsync(CancellationToken cancellationToken)
		{
			try
			{
				var profileStoragePath = Path.Combine(_envPath, _storagePath);

				// Add 3 App Users
				for (int y = 0; y < 3; y++)
				{
					var waiter = new ApplicationUser()
					{
						FirstName = "Ivan",
						LastName = $"Ivanov {y.ToString("D2")}",
						UserName = $"user{y.ToString("D2")}@local.com",
						Email = $"user{y.ToString("D2")}@local.com",
						EmailConfirmed = true
					};
					waiter.ProfilePicture = Task.Run(() => RandomFaceGenerator.GetRandomFaceAsync(profileStoragePath)).Result;
					var waiterResult = await _userManager.CreateAsync(waiter, _defaultPassword);
					if (waiterResult.Succeeded)
					{
						await _userManager.AddToRoleAsync(waiter, UserRoles.CLIENT_ROLE);
					}
				}
			}
			catch (Exception ex)
			{ }
		}
	}
}
