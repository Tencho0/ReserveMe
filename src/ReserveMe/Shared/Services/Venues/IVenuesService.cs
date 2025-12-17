namespace Shared.Services.Venues
{
	using Shared.Dtos.Venues;

	public interface IVenuesService
	{
		Task<List<VenueAdminDto>> GetVenues();

		Task CreateVenueAsync(VenueCreateDto venueDto);

		Task DeleteVenue(int id);
	}
}
