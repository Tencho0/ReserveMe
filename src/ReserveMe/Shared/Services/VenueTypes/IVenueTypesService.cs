namespace Shared.Services.VenueTypes
{
	using Shared.Dtos.VenueTypes;

	public interface IVenueTypesService
	{
		Task<List<VenueTypeDto>> GetAllAsync();
	}
}
