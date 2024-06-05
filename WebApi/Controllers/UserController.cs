using Bussiness.Dtos.Request;
using Bussiness.Utils;
using DataAccess.IdentityConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UserController : ControllerBase
	{

		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly TokenUtils _tokenUtils;
		public UserController(UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager,
			TokenUtils tokenUtils)
		{
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._tokenUtils = tokenUtils;
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
		{
			var userName = HttpContext.User.Identity.Name;

			var user = await _userManager.FindByNameAsync(loginRequest.UserName);
			if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
			{
				var userRoles = await _userManager.GetRolesAsync(user);

				var authClaims = new List<Claim>
				{
					new Claim("Name", user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, user.Id),
				};
				if (userRoles.Count == 0)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, "User"));
				}
				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				var token = _tokenUtils.GetToken(authClaims);
				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					expired = token.ValidTo
				});
			}
			return BadRequest(new
			{
				message = "Login failed"
			});
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			var userExists = await _userManager.FindByNameAsync(model.Username);
			if (userExists != null)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

			IdentityUser user = new IdentityUser()
			{
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.Username
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
			}
			return Ok(new { Status = "Success", Message = "User created successfully!" });
		}

		[Authorize(Roles = UserRoles.Admin)]
		[HttpPost("staffs/register")]
		public async Task<IActionResult> CreateStaff([FromBody] RegisterRequest model)
		{
			var userExists = await _userManager.FindByNameAsync(model.Username);
			if (userExists != null)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

			IdentityUser user = new IdentityUser()
			{
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.Username
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
			}
			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

			if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
			{
				await _userManager.AddToRoleAsync(user, UserRoles.Staff);
			}
			return Ok(new { Status = "Success", Message = "User created successfully!" });
		}

		[Authorize(Roles = UserRoles.Admin)]
		[HttpPost("doctors/register")]
		public async Task<IActionResult> CreateDoctor([FromBody] RegisterRequest model)
		{
			var userExists = await _userManager.FindByNameAsync(model.Username);
			if (userExists != null)
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

			IdentityUser user = new IdentityUser()
			{
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.Username
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
			}
			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

			if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
			{
				await _userManager.AddToRoleAsync(user, UserRoles.Doctor);
			}
			return Ok(new { Status = "Success", Message = "User created successfully!" });
		}


		[HttpPost]
		public async Task CreateAdmin()
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var userName = configuration["Account:UserName"];
			var email = configuration["Account:Email"];
			var password = configuration["Account:Password"];
			var userExists = await _userManager.FindByNameAsync(userName);
			if (userExists != null)
				return;

			IdentityUser user = new IdentityUser()
			{
				Email = email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = userName
			};
			var result = await _userManager.CreateAsync(user, password);
			if (!result.Succeeded)
				return;
			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

			if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
			{
				await _userManager.AddToRoleAsync(user, UserRoles.Admin);
			}
			return;
		}

		[HttpGet("doctors")]
		[Authorize(Roles = UserRoles.Admin)]
		public List<IdentityUser> GetDoctors()
		{
			var users = _userManager.Users.ToList();
			var doctors = users.Where(u => _userManager.IsInRoleAsync(u, UserRoles.Doctor.ToString()).Result).ToList();
			return doctors;
		}
	}
}
