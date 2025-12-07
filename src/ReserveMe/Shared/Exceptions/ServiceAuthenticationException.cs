namespace Shared.Exceptions
{
	public class ServiceAuthenticationException : Exception
	{
		public ServiceAuthenticationException()
		{ }

		public ServiceAuthenticationException(string message) : base(message)
		{ }
	}
}
