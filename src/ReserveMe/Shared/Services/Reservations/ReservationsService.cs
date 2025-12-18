namespace Shared.Services.Reservations
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Shared.Dtos.Reservations;
	using Shared.Providers;

	public class ReservationsService : IReservationsService
	{
		private readonly IApiProvider _provider;

		public ReservationsService(IApiProvider apiProvider)
		{
			this._provider = apiProvider;
		}

		#region GET

		public async Task<List<ReservationDto>> GetReservations(int venueId)
		{
			try
			{
				object[] uriParams = new object[]
				{
					venueId
				};

				var result = await _provider.GetAsync<List<ReservationDto>>(Endpoints.GetReservations + "/{0}", uriParams, null, null);

				return result;
			}
			catch (Exception ex)
			{
				return new List<ReservationDto>();
			}
		}

		#endregion
	}
}
