namespace Shared.Services.Reservations
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Common.Enums;
	using Shared.Dtos.Reservations;
	using Shared.Dtos.Venues;
	using Shared.Providers;
	using Shared.Requests.Reservations;
	using Shared.Requests.Venues;

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

		public async Task<List<ReservationForClientDto>> GetReservationsByClientId(string userId)
		{
			try
			{
				object[] uriParams = new object[]
				{
					userId
				};

				var result = await _provider.GetAsync<List<ReservationForClientDto>>(Endpoints.GetReservationsByClientId + "/{0}", uriParams, null, null);

				return result;
			}
			catch (Exception ex)
			{
				return new List<ReservationForClientDto>();
			}
		}

		#endregion

		#region POST

		public async Task CreateReservationAsync(ReservationDto reservationDto)
		{
			try
			{
				//TODO: Automapper
				//var model = _mapper.Map<SaveReservationRequest>(reservationDto);
				var model = new SaveReservationRequest()
				{
					UserId = reservationDto.UserId,
					VenueId = reservationDto.VenueId,
					TableNumber = reservationDto.TableNumber,
					GuestsCount = reservationDto.GuestsCount,
					ContactName = reservationDto.ContactName,
					ContactPhone = reservationDto.ContactPhone,
					ContactEmail = reservationDto.ContactEmail,
					ReservationTime = reservationDto.ReservationTime,
					//CreatedAt = reservationDto.CreatedAt,
					Status = (int)reservationDto.Status
				};

				await _provider.PostAsync<SaveReservationRequest, object>(Endpoints.CreateReservation, model, null);
			}
			catch (Exception ex)
			{
				//TODO: Log errors
				//this._logger.LogError(ex.Message);
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
