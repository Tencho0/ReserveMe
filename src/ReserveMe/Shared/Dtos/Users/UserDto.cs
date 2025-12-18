namespace Shared.Dtos.Users
{
	public class UserDto
	{
		public int? VenueId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public bool IsActive { get; set; } = true;

		public string PhoneNumber { get; set; }
	}
}
