namespace ReserveMe.Server.Controllers
{
	using System.IdentityModel.Tokens.Jwt;
	using System.Security.Claims;
	using System.Text;
	using Common;
	using Domain.Entities;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.IdentityModel.Tokens;
	using Shared.Authorization;
	using Shared.Requests;

	public class AuthController : ApiControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;

		public AuthController(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
		}

		[HttpPost("login")]
		public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginUserRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return Unauthorized("Invalid credentials");

			var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
			if (!validPassword)
				return Unauthorized("Invalid credentials");

			var token = await GenerateJwtTokenAsync(user);
			return Ok(token);
		}

		[HttpPost("register")]
		public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterUserRequest request)
		{
			var existing = await _userManager.FindByEmailAsync(request.Email);
			if (existing != null)
				return BadRequest("Email is already registered.");

			var user = new ApplicationUser
			{
				FirstName = request.FirstName,
				LastName = request.LastName,
				UserName = request.Email,
				Email = request.Email
			};

			var createResult = await _userManager.CreateAsync(user, request.Password);
			if (!createResult.Succeeded)
			{
				var errors = createResult.Errors.Select(e => e.Description);
				return BadRequest(new { Errors = errors });
			}

			await _userManager.AddToRoleAsync(user, UserRoles.CLIENT_ROLE);

			var token = await GenerateJwtTokenAsync(user);
			return Ok(token);
		}

		private async Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user)
		{
			var jwtSection = _configuration.GetSection("Jwt");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var roles = await _userManager.GetRolesAsync(user);

			var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
			new("firstName", user.FirstName ?? ""),
			new("lastName", user.LastName ?? "")
		};

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiresInMinutes"]!));

			var token = new JwtSecurityToken(
				issuer: jwtSection["Issuer"],
				audience: jwtSection["Audience"],
				claims: claims,
				expires: expires,
				signingCredentials: creds);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return new AuthResponse
			{
				Token = tokenString,
				ExpiresAt = expires
			};
		}
	}
}
