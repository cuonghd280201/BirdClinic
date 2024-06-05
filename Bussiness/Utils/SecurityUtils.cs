using Common.ExceptionHandler.Exceptions;
using DataAccess.IdentityConfig;
using Microsoft.Extensions.Caching.Memory;

namespace Bussiness.Utils
{
	public class SecurityUtils
	{
		private readonly IMemoryCache _cache;

		public SecurityUtils(IMemoryCache cache)
		{
			_cache = cache;
		}

		public String UserId()
		{
			if (_cache.TryGetValue("UserId", out string UserId))
			{
				return UserId;
			}
			throw new BadRequestException("UserId Not Found");
		}


		public bool IsStaff()
		{
			if (_cache.TryGetValue("Role", out string role))
			{
				return role.Equals(UserRoles.Staff, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public bool IsAdmin()
		{
			if (_cache.TryGetValue("Role", out string role))
			{
				return role.Equals(UserRoles.Admin, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public bool IsDoctor()
		{
			if (_cache.TryGetValue("Role", out string role))
			{
				return role.Equals(UserRoles.Doctor, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public bool IsUser()
		{
			if (_cache.TryGetValue("Role", out string role))
			{
				return role.Equals(UserRoles.User, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}


		public String GetCurrentRole()
		{
			if (_cache.TryGetValue("Role", out string roleName))
			{
				return roleName;
			}
			throw new BadRequestException("Role not found");
		}
	}
}
