namespace Shared
{
	public class Endpoints
	{
		public static readonly string BaseAddress = "https://localhost:7118/";

		public static readonly string BaseAddressApi = $"{BaseAddress}api/";

		#region Users

		public static string RegisterUser = $"{BaseAddressApi}auth/register";

		public static string LoginUser = $"{BaseAddressApi}auth/login";

		#endregion

		#region Venues

		public static string GetVenues = $"{BaseAddressApi}venues/getAll";

		public static string CreateVenue = $"{BaseAddressApi}venues/create";

		public static string DeleteVenue = $"{BaseAddressApi}venues/delete";

		#endregion

		#region VenueTypes

		public static string GetVenueTypes = $"{BaseAddressApi}venueTypes/getAll";

		#endregion

		#region Media

		public static string UploadImage = $"{BaseAddressApi}media/upload";

		#endregion

		public static string Reservations = $"{BaseAddressApi}reservations/reserve";
	}
}
