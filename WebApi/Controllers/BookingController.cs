using Bussiness;
using Bussiness.Dtos.Request;
using Bussiness.Error;
using Common.ExceptionHandler.Exceptions;
using DataAccess.IdentityConfig;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Controllers
{
	[Route("api/bookings")]
	[ApiController]
	[Authorize]
	public class BookingController : ControllerBase
	{
		private readonly BookingSvc _bookingService;

		public BookingController(BookingSvc bookingService)
		{
			_bookingService = bookingService;
		}

		//Get booking of user or get all
		[HttpGet]
		public List<Booking> GetBookingByUserId([FromQuery] int page, [FromQuery] int size)
		{
			var userId = HttpContext.Session.GetString("UserId");
			var role = HttpContext.Session.GetString("Role");
			if (userId == null || role == null)
			{
				throw new BadRequestException(ErrorCode.TOKEN_IS_MISSING);
			}
			if (role.Equals(UserRoles.User))
			{
				return _bookingService.GetAllByUserId(Guid.Parse(userId), page, size);
			}
			else if (role.Equals(UserRoles.Staff))
			{
				return _bookingService.GetAll(page, size);
			}
			else
			{
				throw new BadRequestException(ErrorCode.MISSING_PERMISSON);
			}
		}

		[HttpGet("{bookingId}/details")]
		public List<BookingDetail> GetBookingDetail([FromRoute] Guid bookingId)
		{
			return _bookingService.GetBookingDetail(bookingId);
		}

		//Get all service in booking
		[HttpGet("{bookingId}/services")]
		public List<Service> GetBookingService([FromRoute] Guid bookingId)
		{
			return _bookingService.GetAllService(bookingId);
		}

		//Create booking and add service
		[HttpPost("services")]
		[Authorize(Roles = UserRoles.User)]
		public void AddService([FromBody] AddServiceRequest request)
		{
			var userId = HttpContext.Session.GetString("UserId");
			if (userId == null)
			{
				throw new BadRequestException(ErrorCode.TOKEN_IS_MISSING);
			}
			request.UserId = Guid.Parse(userId);
			_bookingService.AddService(request);
		}

		//Add service to existed booking
		[HttpPost("{bookingId}/services/{serviceId}")]
		[Authorize(Roles = UserRoles.User)]
		public void AddServiceForExistBooking([FromRoute] Guid bookingId, [FromRoute] Guid serviceId)
		{
			_bookingService.AddServiceExistBooking(bookingId, serviceId);
		}

		//Assign doctor to booking
		[HttpPost]
		[ExcludeFromCodeCoverage]
		public void AssignDoctorBooking([FromQuery] Guid doctorId, [FromQuery] Guid bookingId)
		{
			_bookingService.AssignDoctorForBooking(doctorId, bookingId);
		}

		[HttpPut("assign")]
		[Authorize(Roles = UserRoles.Staff)]
		public void AssginService([FromForm] Guid bookingId, [FromForm] Guid bookingDetailId, [FromForm] Guid doctorId)
		{
			_bookingService.AssignBookingDetail(bookingId, bookingDetailId, doctorId);
		}
	}
}
