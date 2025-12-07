namespace Shared.Exceptions
{
	public class ApiValidationException : HttpRequestException
	{
		public System.Net.HttpStatusCode HttpCode { get; private set; } = System.Net.HttpStatusCode.BadRequest;

		public IDictionary<string, string[]>? Failures { get; private set; } = new Dictionary<string, string[]>();

		public ApiValidationException(IDictionary<string, string[]>? failures)
		{
			Failures = failures;
		}
	}
}
