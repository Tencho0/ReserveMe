namespace Shared.Exceptions
{
	public class ApiRequestException : HttpRequestException
	{
		public System.Net.HttpStatusCode HttpCode { get; private set; }

		public ApiRequestException(System.Net.HttpStatusCode code) : this(code, string.Empty, new Exception())
		{ }

		public ApiRequestException(System.Net.HttpStatusCode code, string? message) : this(code, message, new Exception())
		{ }

		public ApiRequestException(System.Net.HttpStatusCode code, string? message, Exception inner) : base(message,
			inner)
		{
			HttpCode = code;
		}
	}
}
