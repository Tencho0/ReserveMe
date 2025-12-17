namespace Shared.Services.VenueTypes
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Shared.Dtos.Venues;
	using Shared.Dtos.VenueTypes;
	using Shared.Providers;

	public class VenueTypesService : IVenueTypesService
	{
		private readonly IApiProvider _provider;

		public VenueTypesService(IApiProvider apiProvider)
		{
			this._provider = apiProvider;
		}

		#region GET
		public async Task<List<VenueTypeDto>> GetAllAsync()
		{
			try
			{
				var result = await _provider.GetAsync<List<VenueTypeDto>>(Endpoints.GetVenueTypes, null, null);

				return result;
			}
			catch (Exception ex)
			{
				return new List<VenueTypeDto>();
			}
		}
		#endregion
	}
}
