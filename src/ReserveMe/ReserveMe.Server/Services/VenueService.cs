using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Domain.Entities;
using Domain.Enums;
using Shared.Dtos;
using ReserveMe.Server.Interfaces;

namespace ReserveMe.Server.Services
{
    public class VenueService : IVenueService
    {
        private readonly ApplicationDbContext _context;

        public VenueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VenueSearchResultDto> SearchVenuesAsync(VenueSearchRequestDto request, string? userId = null)
        {
            var query = _context.Venues
                .Include(v => v.VenueType)
                .Include(v => v.VenueReviews)
                .Include(v => v.Reservations)
                .Include(v => v.Favorites)
                .Where(v => v.IsActive && !v.IsDeleted)
                .AsQueryable();

            // Search by name
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(v => v.Name.ToLower().Contains(term) ||
                                         (v.Description != null && v.Description.ToLower().Contains(term)));
            }

            // Filter by venue types (multiple)
            if (request.VenueTypeIds != null && request.VenueTypeIds.Any())
            {
                query = query.Where(v => v.VenueTypeId.HasValue && request.VenueTypeIds.Contains(v.VenueTypeId.Value));
            }

            var venues = await query.ToListAsync();

            // Calculate distance and filter by radius
            var venuesWithDistance = venues.Select(v => new
            {
                Venue = v,
                Distance = CalculateDistance(
                    request.UserLatitude ?? 42.6977, // Default Sofia coordinates
                    request.UserLongitude ?? 23.3219,
                    v.Latitude,
                    v.Longitude)
            })
            .Where(v => v.Distance <= request.RadiusKm)
            .ToList();

            // Sort
            var sortedVenues = request.SortBy?.ToLower() switch
            {
                "distance" => venuesWithDistance.OrderBy(v => v.Distance),
                "reservations" => venuesWithDistance.OrderByDescending(v => v.Venue.Reservations.Count),
                _ => venuesWithDistance.OrderByDescending(v =>
                    v.Venue.VenueReviews.Any() ? v.Venue.VenueReviews.Average(r => r.Rating ?? 0) : 0)
            };

            var totalCount = sortedVenues.Count();

            // Paginate
            var pagedVenues = sortedVenues
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var venueDtos = pagedVenues.Select(v => MapToSearchDto(v.Venue, v.Distance, userId)).ToList();

            return new VenueSearchResultDto
            {
                Venues = venueDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                HasMore = request.Page * request.PageSize < totalCount
            };
        }

        public async Task<VenueDetailDto?> GetVenueByIdAsync(int venueId, string? userId = null)
        {
            var venue = await _context.Venues
                .Include(v => v.VenueType)
                .Include(v => v.VenueReviews)
                    .ThenInclude(r => r.User)
                .Include(v => v.Reservations)
                .Include(v => v.Tables)
                .Include(v => v.Favorites)
                .FirstOrDefaultAsync(v => v.Id == venueId && v.IsActive && !v.IsDeleted);

            if (venue == null) return null;

            var rating = venue.VenueReviews.Any()
                ? venue.VenueReviews.Average(r => r.Rating ?? 0)
                : 0;

            return new VenueDetailDto
            {
                Id = venue.Id,
                Name = venue.Name,
                Description = venue.Description,
                VenueTypeName = venue.VenueType?.Name,
                VenueTypeId = venue.VenueTypeId,
                Longitude = venue.Longitude,
                Latitude = venue.Latitude,
                LogoUrl = venue.LogoUrl,
                ImageUrl = venue.ImageUrl,
                Rating = Math.Round(rating, 1),
                ReviewCount = venue.VenueReviews.Count,
                TotalReservations = venue.Reservations.Count,
                TotalTables = venue.Tables.Count(t => t.IsActive),
                AvailableTables = venue.Tables.Count(t => t.IsActive && t.Status == TableStatus.Available),
                IsFavorite = !string.IsNullOrEmpty(userId) && venue.Favorites.Any(f => f.UserId == userId),
                RecentReviews = venue.VenueReviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new VenueReviewDto
                    {
                        Id = r.Id,
                        UserName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : "Anonymous",
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList()
            };
        }

        public async Task<IEnumerable<VenueTypeDto>> GetVenueTypesAsync()
        {
            var types = await _context.VenueTypes
                .OrderBy(t => t.Name)
                .Select(t => new VenueTypeDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return types;
        }

        public async Task<VenueSearchDto> CreateVenueAsync(CreateVenueDto dto)
        {
            var venue = new Venue
            {
                Name = dto.Name,
                Description = dto.Description,
                VenueTypeId = dto.VenueTypeId,
                Longitude = dto.Longitude,
                Latitude = dto.Latitude,
                LogoUrl = dto.LogoUrl,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(venue).Reference(v => v.VenueType).LoadAsync();

            return MapToSearchDto(venue, 0, null);
        }

        public async Task<bool> ToggleFavoriteAsync(int venueId, string userId)
        {
            var existingFavorite = await _context.VenueFavorites
                .FirstOrDefaultAsync(f => f.VenueId == venueId && f.UserId == userId);

            if (existingFavorite != null)
            {
                _context.VenueFavorites.Remove(existingFavorite);
                await _context.SaveChangesAsync();
                return false; // Removed from favorites
            }
            else
            {
                var favorite = new VenueFavorite
                {
                    VenueId = venueId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.VenueFavorites.Add(favorite);
                await _context.SaveChangesAsync();
                return true; // Added to favorites
            }
        }

        public async Task<IEnumerable<VenueSearchDto>> GetFavoriteVenuesAsync(string userId)
        {
            var favorites = await _context.VenueFavorites
                .Include(f => f.Venue)
                    .ThenInclude(v => v.VenueType)
                .Include(f => f.Venue)
                    .ThenInclude(v => v.VenueReviews)
                .Include(f => f.Venue)
                    .ThenInclude(v => v.Reservations)
                .Where(f => f.UserId == userId && f.Venue.IsActive && !f.Venue.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return favorites.Select(f => MapToSearchDto(f.Venue, 0, userId));
        }

        private VenueSearchDto MapToSearchDto(Venue venue, double distance, string? userId)
        {
            var rating = venue.VenueReviews?.Any() == true
                ? venue.VenueReviews.Average(r => r.Rating ?? 0)
                : 0;

            return new VenueSearchDto
            {
                Id = venue.Id,
                Name = venue.Name,
                Description = venue.Description,
                VenueTypeName = venue.VenueType?.Name,
                VenueTypeId = venue.VenueTypeId,
                Longitude = venue.Longitude,
                Latitude = venue.Latitude,
                LogoUrl = venue.LogoUrl,
                ImageUrl = venue.ImageUrl,
                Rating = Math.Round(rating, 1),
                ReviewCount = venue.VenueReviews?.Count ?? 0,
                TotalReservations = venue.Reservations?.Count ?? 0,
                Distance = Math.Round(distance, 1),
                IsFavorite = !string.IsNullOrEmpty(userId) && venue.Favorites?.Any(f => f.UserId == userId) == true,
                Address = null // Would need geocoding service to get address from coordinates
            };
        }

        // Haversine formula for distance calculation
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in km

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    }
}