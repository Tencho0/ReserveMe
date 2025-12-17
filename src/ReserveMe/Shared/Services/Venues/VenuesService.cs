namespace Shared.Services.Venues
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Shared.Dtos.Venues;
	using Shared.Providers;
	using Shared.Requests.Venues;

	public class VenuesService : IVenuesService
	{
		private readonly IApiProvider _provider;

		public VenuesService(IApiProvider apiProvider)
		{
			this._provider = apiProvider;
		}

		#region GET

		public async Task<List<VenueAdminDto>> GetVenues()
		{
			try
			{
				var result = await _provider.GetAsync<List<VenueAdminDto>>(Endpoints.GetVenues, null, null);

				return result;
			}
			catch (Exception ex)
			{
				return new List<VenueAdminDto>();
			}
		}

		#endregion

		#region POST

		public async Task CreateVenueAsync(VenueCreateDto venueDto)
		{
			try
			{
				//TODO: Automapper
				//var model = _mapper.Map<SaveVenueRequest>(venueDto);
				var model = new SaveVenueRequest()
				{
					Name = venueDto.Name,
					Description = venueDto.Description,
					VenueTypeId = venueDto.VenueTypeId,
					Longitude = venueDto.Longitude,
					Latitude = venueDto.Latitude,
					LogoUrl = venueDto.LogoUrl,
					ImageUrl = venueDto.ImageUrl
				};

				await _provider.PostAsync<SaveVenueRequest, object>(Endpoints.CreateVenue, model, null);
			}
			catch (Exception ex)
			{
				//TODO: Log errors
				//this._logger.LogError(ex.Message);
			}
		}

		#endregion

		#region DELETE

		public async Task DeleteVenue(int id)
		{
			try
			{
				var model = new DeleteVenueRequest()
				{
					VenueId = id
				};

				await _provider.DeleteAsync<DeleteVenueRequest, object>(Endpoints.DeleteVenue, model);
			}
			catch (Exception ex)
			{
				//TODO: Log errors
				//this._logger.LogError(ex.Message);
			}
		}

		#endregion
	}
}
