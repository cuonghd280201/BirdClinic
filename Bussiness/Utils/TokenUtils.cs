using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bussiness.Utils
{
	public class TokenUtils
	{
		private readonly IConfiguration _configuration;

		public TokenUtils(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(3),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
				);
			return token;
		}

		public static TokenInfo Decode(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var tokenAfter = handler.ReadJwtToken(token);
			var username = tokenAfter.Claims.ToList()[0].Value;
			var userId = tokenAfter.Claims.ToList()[1].Value;
			var role = tokenAfter.Claims.ToList()[2].Value;
			var tokenInfo = new TokenInfo
			{
				Name = username,
				Role = role,
				UserId = userId
			};
			return tokenInfo;
		}

	}
}
