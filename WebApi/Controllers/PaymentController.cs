using Bussiness;
using DataAccess.IdentityConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Route("api/payments")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly PaymentService _paymentSvc;
		public PaymentController(PaymentService paymentSvc)
		{
			_paymentSvc = paymentSvc;
		}

		//Request to confirm payment
		[HttpPut("booking/{bookingId}/request")]
		[Authorize(Roles = UserRoles.User)]
		public void RequestPayment(Guid bookingId)
		{
			_paymentSvc.requestPayment(bookingId);
		}

		//Confirm booking payment
		[HttpPut("booking/{bookingId}/confirm")]
		[Authorize(Roles = UserRoles.Admin)]
		public void ConfirmPayment(Guid bookingId)
		{
			_paymentSvc.bookingPayment(bookingId);
		}
	}
}
