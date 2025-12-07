namespace Shared.Dtos
{
	using System.ComponentModel.DataAnnotations;

	public class RegisterUserDto
	{
		[Required]
		public string FirstName { get; set; } = null!;

		[Required]
		public string LastName { get; set; } = null!;

		[Required]
		public string Email { get; set; } = null!;

		[Required]
		public string Password { get; set; } = null!;

		[Compare("Password")]
		public string ConfirmPassword { get; set; } = null!;
	}
}
