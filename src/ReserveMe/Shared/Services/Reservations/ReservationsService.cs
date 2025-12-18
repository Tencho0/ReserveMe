namespace Shared.Services.Reservations
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Common.Enums;
	using Shared.Dtos.Reservations;
	using Shared.Providers;
	using Shared.Requests.Reservations;

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

		#region PUT

		public async Task<bool> ChangeReservationStatus(int reservationId, ReservationStatus newStatus)
		{
			try
			{
				var model = new UpdateReservationStatusRequest()
				{
					ReservationId = reservationId,
					NewStatus = (int)newStatus
				};

				return await _provider.PutAsync<UpdateReservationStatusRequest, bool>(Endpoints.UpdateReservationStaus, model, null);
			}
			catch (Exception ex)
			{
				//TODO: Log error
				//_logger.LogError(ex.Message);

				return false;
			}
		}

		#endregion
	}
}
