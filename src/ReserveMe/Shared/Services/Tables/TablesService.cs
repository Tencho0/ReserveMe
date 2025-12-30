namespace Shared.Services.Tables
{
	using Shared.Dtos.Tables;
	using Shared.Providers;

	public class TablesService : ITablesService
	{
		private readonly IApiProvider _provider;

		public TablesService(IApiProvider apiProvider)
		{
			this._provider = apiProvider;
		}

		#region GET

		public async Task<List<TableDto>> GetTablesByVenueId(int venueId)
		{
			try
			{
				object[] uriParams = new object[]
				{
					venueId
				};

				var result = await _provider.GetAsync<List<TableDto>>(Endpoints.GetTablesByVenueId + "/{0}", uriParams, null, null);

				return result;
			}
			catch (Exception ex)
			{
				return new List<TableDto>();
			}
		}

		#endregion
	}
}
