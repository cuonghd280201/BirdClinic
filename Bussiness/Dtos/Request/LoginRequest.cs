using System.ComponentModel.DataAnnotations;

namespace Bussiness.Dtos.Request
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "UserName is required")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }
	}
}
