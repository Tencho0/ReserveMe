namespace Shared.Exceptions
{
	using System.Net;

	public class HttpRequestExceptionEx : HttpRequestException
	{
		public HttpStatusCode HttpCode { get; }

		public HttpRequestExceptionEx(HttpStatusCode code) : this(code, string.Empty, new Exception())
		{ }

		public HttpRequestExceptionEx(HttpStatusCode code, string message) : this(code, string.Empty, new Exception())
		{ }

		public HttpRequestExceptionEx(HttpStatusCode code, string message, Exception inner) : base(message, inner)
		{
			HttpCode = code;
		}
	}
}
