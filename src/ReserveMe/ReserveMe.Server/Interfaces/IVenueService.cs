using Shared.Dtos;

namespace ReserveMe.Server.Interfaces
{
    public interface IVenueService
    {
        Task<VenueSearchResultDto> SearchVenuesAsync(VenueSearchRequestDto request, string? userId = null);

        Task<VenueDetailDto?> GetVenueByIdAsync(int venueId, string? userId = null);

        Task<IEnumerable<VenueTypeDto>> GetVenueTypesAsync();

        Task<VenueSearchDto> CreateVenueAsync(CreateVenueDto dto);

        Task<bool> ToggleFavoriteAsync(int venueId, string userId);

        Task<IEnumerable<VenueSearchDto>> GetFavoriteVenuesAsync(string userId);
    }
}