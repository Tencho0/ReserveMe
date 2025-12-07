namespace Shared.Requests
{
	using System.ComponentModel.DataAnnotations;

	//TODO: Automapper
	public class LoginUserRequest //  : IMapFrom<LoginUserDto>
	{
		[Required]
		public string Email { get; set; } = null!;

		[Required]
		public string Password { get; set; } = null!;

		//public void Mapping(Profile profile)
		//{
		//	profile.CreateMap<LoginUserDto, LoginUserRequest>();
		//}
	}
}
