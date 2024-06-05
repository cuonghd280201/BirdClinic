using Bussiness.Utils;
using Common.ExceptionHandler.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace Bussiness.Middleware
{
	public class TokenMiddleware : IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
				if (context.Request.Headers.ContainsKey("Authorization"))
				{
					string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
					var tokenInfo = ExtractTokenInfoFromToken(token);
					if (!string.IsNullOrEmpty(tokenInfo.UserId))
					{
						var session = context.Session;
						session.SetString("UserId", tokenInfo.UserId);
						session.SetString("Role", tokenInfo.Role);
						session.SetString("UserName", tokenInfo.Name);
						IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
						cache.Set("UserId", tokenInfo.UserId);
						cache.Set("Role", tokenInfo.Role);
						cache.Set("UserName", tokenInfo.Name);
					}
					else
					{
						//context.Response.StatusCode = StatusCodes.Status401Unauthorized;
						throw new BadRequestException("Token invalid");
					}
				}
			await next(context);
		}

		private TokenInfo ExtractTokenInfoFromToken(string token)
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
