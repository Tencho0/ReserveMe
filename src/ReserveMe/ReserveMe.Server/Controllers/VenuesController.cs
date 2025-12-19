using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shared.Dtos;
using ReserveMe.Server.Interfaces;

namespace ReserveMe.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        /// <summary>
        /// Search venues with filters
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<VenueSearchResultDto>> SearchVenues(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] int radiusKm = 5,
            [FromQuery] string? search = null,
            [FromQuery] List<int>? typeIds = null,
            [FromQuery] string sortBy = "rating",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = new VenueSearchRequestDto
            {
                UserLatitude = latitude,
                UserLongitude = longitude,
                RadiusKm = radiusKm,
                SearchTerm = search,
                VenueTypeIds = typeIds,
                SortBy = sortBy,
                Page = page,
                PageSize = pageSize
            };

            var result = await _venueService.SearchVenuesAsync(request, userId);
            return Ok(result);
        }

        /// <summary>
        /// Get all venues (simple list)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<VenueSearchResultDto>> GetAllVenues(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = new VenueSearchRequestDto
            {
                RadiusKm = 1000, // Large radius to get all
                Page = page,
                PageSize = pageSize
            };

            var result = await _venueService.SearchVenuesAsync(request, userId);
            return Ok(result);
        }

        /// <summary>
        /// Get venue by ID
        /// </summary>
        [HttpGet("{venueId:int}")]
        public async Task<ActionResult<VenueDetailDto>> GetVenue(int venueId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var venue = await _venueService.GetVenueByIdAsync(venueId, userId);
            if (venue == null)
                return NotFound();

            return Ok(venue);
        }

        /// <summary>
        /// Get all venue types
        /// </summary>
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<VenueTypeDto>>> GetVenueTypes()
        {
            var types = await _venueService.GetVenueTypesAsync();
            return Ok(types);
        }

        /// <summary>
        /// Create new venue
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<VenueSearchDto>> CreateVenue([FromBody] CreateVenueDto dto)
        {
            var venue = await _venueService.CreateVenueAsync(dto);
            return CreatedAtAction(nameof(GetVenue), new { venueId = venue.Id }, venue);
        }

        /// <summary>
        /// Toggle venue favorite status
        /// </summary>
        [HttpPost("{venueId:int}/favorite")]
        [Authorize]
        public async Task<ActionResult<object>> ToggleFavorite(int venueId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isFavorite = await _venueService.ToggleFavoriteAsync(venueId, userId);
            return Ok(new { isFavorite });
        }

        /// <summary>
        /// Get user's favorite venues
        /// </summary>
        [HttpGet("favorites")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<VenueSearchDto>>> GetFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var favorites = await _venueService.GetFavoriteVenuesAsync(userId);
            return Ok(favorites);
        }
    }
}