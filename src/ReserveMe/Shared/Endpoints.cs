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

		public static string Reservations = $"{BaseAddressApi}reservations/reserve";
	}
}
