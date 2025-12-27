namespace Shared.Services.Venues
{
	using Shared.Dtos.Venues;

	public interface IVenuesService
	{
		Task<List<VenueAdminDto>> GetVenues();

		Task<VenueSearchDto> GetVenueById(int venueId);

		Task<VenueDetailsDto> GetMyVenue(int venueId);

		Task<List<VenueSearchDto>> GetVenuesForClient();

		Task CreateVenueAsync(VenueCreateDto venueDto);

		Task DeleteVenue(int id);
	}
}
