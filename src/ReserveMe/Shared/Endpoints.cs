namespace Shared
{
	public class Endpoints
	{
		public static readonly string BaseAddress = "https://localhost:7118/";

		public static readonly string BaseAddressApi = $"{BaseAddress}api/";

		#region Auth

		public static string RegisterUser = $"{BaseAddressApi}auth/register";

		public static string LoginUser = $"{BaseAddressApi}auth/login";

		#endregion

		#region Users

		public static string GetUserByName = $"{BaseAddressApi}users/getUserByName";

		public static string GetUserById = $"{BaseAddressApi}users/getuserbyid";

		#endregion

		#region Venues

		public static string GetVenues = $"{BaseAddressApi}venues/getAll";

		public static string GetVenueById = $"{BaseAddressApi}venues/getVenueById";

		public static string GetVenuesForClient = $"{BaseAddressApi}venues/getVenuesForClient";

		public static string CreateVenue = $"{BaseAddressApi}venues/create";

		public static string DeleteVenue = $"{BaseAddressApi}venues/delete";

		#endregion

		#region VenueTypes

		public static string GetVenueTypes = $"{BaseAddressApi}venueTypes/getAll";

		#endregion

		#region Media

		public static string UploadImage = $"{BaseAddressApi}media/upload";

		#endregion

		#region Reservations

		public static string GetReservations = $"{BaseAddressApi}reservations/getAll";
		public static string GetReservationsByClientId = $"{BaseAddressApi}reservations/getReservationsByClientId";

		public static string CreateReservation = $"{BaseAddressApi}reservations/create";

		public static string UpdateReservationStaus = $"{BaseAddressApi}reservations/updateStaus";

		#endregion

		#region Reviews

		public static string GetReviewsByVenueId = $"{BaseAddressApi}reviews/getReviewsByVenueId";

		public static string CreateReview = $"{BaseAddressApi}reviews/create";

		#endregion

	}
}
