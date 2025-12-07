namespace Shared.Authorization
{
	public class AuthResponse
	{
		public string Token { get; set; } = default!;
		public DateTime ExpiresAt { get; set; }
	}
}
