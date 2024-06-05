using Bussiness.Dtos.Enum;
using Bussiness.Dtos.Request;
using Bussiness.Error;
using Common.ExceptionHandler.Exceptions;
using DataAccess;
using DataAccess.Models;

namespace Bussiness
{
	public class BookingSvc
	{

		private readonly IGenericRep<Booking> _bookingRep;
		private readonly BookingDetailSvc _bookingDetailSvc;
		private readonly ServiceSvc _serviceSvc;
		private readonly ScheduleSvc _scheduleSvc;

		public BookingSvc(
			IGenericRep<Booking> bookingRep,
			BookingDetailSvc bookingDetailService,
			ServiceSvc serviceService,
			ScheduleSvc scheduleSvc)
		{
			_bookingRep = bookingRep;
			_serviceSvc = serviceService;
			_bookingDetailSvc = bookingDetailService;
			_scheduleSvc = scheduleSvc;
		}

		public List<Service> GetAllService(Guid bookingId)
		{
			return _bookingDetailSvc.GetServiceByBooking(bookingId);
		}

		public void CreateBooking(Guid UserId, Guid BookingId, DateTime StartTime, DateTime EndTime, double TotalMoney)
		{
			var booking = new Booking();
			booking.Id = BookingId;
			booking.UserId = UserId;
			booking.TotalMoney = TotalMoney;
			booking.StartTime = StartTime;
			booking.EndTime = EndTime;
			booking.CretedAt = DateTime.UtcNow;
			booking.Status = BookingStatus.WAITING_FOR_PAYMENT.ToString();
			_bookingRep.Create(booking);
		}

		public Booking GetAndCountBalance(Guid bookingId)
		{
			var bookings = _bookingRep.All;
			if (!bookings.Any())
			{
				throw new BadRequestException("Booking not found");
			}
			var booking = bookings.FirstOrDefault(booking => booking.Id == bookingId);
			if (ReferenceEquals(booking, null))
			{
				throw new BadRequestException("Booking not found");
			}
			var serviceIds = _bookingDetailSvc.GetByBookingId(booking.Id).Select(it => it.ServiceId).ToList();
			if (!serviceIds.Any())
			{
				return booking;
			}
			var services = _serviceSvc.GetByIds(serviceIds);
			if (!services.Any())
			{
				return booking;
			}
			booking.EndTime = DateTime.Now;
			booking.TotalMoney = 0;
			services.ForEach(service =>
			{
				booking.EndTime = booking.EndTime.AddHours(service.EstimateTime);
				booking.TotalMoney = booking.TotalMoney + service.Price;
			});
			_bookingRep.Update(booking);
			return booking;
		}

		//User - Get all booking
		public List<Booking> GetAllByUserId(Guid userId, int page, int size)
		{
			var bookings = _bookingRep.All;
			if (bookings == null)
			{
				return new List<Booking>();
			}
			var result = bookings.Where(booking => booking.UserId == userId).Skip((page - 1) * size);
			if (!result.Any())
			{
				return new List<Booking>();
			}
			return result.ToList();
		}

		//Staff - Get all booking
		public List<Booking> GetAll(int page, int size)
		{
			var bookings = _bookingRep.All;
			if (bookings == null)
			{
				return new List<Booking>();
			}
			var result = bookings.Skip((page - 1) * size);
			if (result == null)
			{
				return new List<Booking>();
			}
			return result.ToList();
		}

		public Booking GetById(Guid bookingId)
		{
			var bookings = _bookingRep.All;
			if (bookings == null)
			{
				throw new BadRequestException("Booking not found");
			}
			var result = bookings.FirstOrDefault(booking => booking.Id == bookingId);
			if (result == null)
			{
				throw new BadRequestException("Booking not found");
			}
			return result;
		}

		public void Update(Booking booking)
		{
			_bookingRep.Update(booking);
		}

		public void AddServiceExistBooking(Guid bookingId, Guid serviceId)
		{
			//Check and get booking
			var booking = GetById(bookingId);
			//Check booking is waiting for payment
			if(!booking.Status.Equals(BookingStatus.WAITING_FOR_PAYMENT.ToString()))
			{
				throw new BadRequestException("Status is not accept");
			}
			//Check and get service
			//Check exist service
			var service = _serviceSvc.GetById(serviceId);
			if (booking.EndTime.AddHours(service.EstimateTime).Hour > 17)
			{
				throw new BadRequestException(ErrorCode.TIME_END_ERROR);
			}
			//Update booking
			booking.TotalMoney = booking.TotalMoney + service.Price;
			booking.EndTime = booking.EndTime.AddHours(service.EstimateTime);
			Update(booking);
			//Crate new booking detail
			var bookingDetails = new BookingDetail();
			bookingDetails.ServicePrice = service.Price;
			bookingDetails.ServiceId = service.Id;
			bookingDetails.BookingId = bookingId;
			_bookingDetailSvc.Craete(bookingDetails);
		}

		public void AddService(AddServiceRequest request)
		{
			if(request.StartAt.Hour < 7)
			{
				throw new BadRequestException(ErrorCode.TIME_START_ERROR);
			}
			//Check exist service
			var service = _serviceSvc.GetById(request.ServiceId);
			if (request.StartAt.AddHours(service.EstimateTime).Hour > 17)
			{
				throw new BadRequestException(ErrorCode.TIME_END_ERROR);
			}
				//Create booking
				var bookingId = Guid.NewGuid();
			CreateBooking(
				request.UserId,
				bookingId,
				request.StartAt,
				DateTime.Now.AddHours(service.EstimateTime),
				service.Price);
			//Create booking details
			var bookingDetails = new BookingDetail();
			bookingDetails.ServicePrice = service.Price;
			bookingDetails.ServiceId = service.Id;
			bookingDetails.BookingId = bookingId;
			_bookingDetailSvc.Craete(bookingDetails);
		}

		public void AssignDoctorForBooking(Guid bookingId, Guid doctorId)
		{
			var booking = GetById(bookingId);
			if (booking.Status.Equals(BookingStatus.WAITING_FOR_PAYMENT.ToString()))
			{
				throw new BadRequestException(ErrorCode.NOT_PAYMENT);
			}
			if (booking.Status.Equals(BookingStatus.WAITING_FOR_CONFIRM.ToString()))
			{
				throw new BadRequestException(ErrorCode.BOOKING_WAITING_FOR_CONFIRM);
			}
			if (booking.Status.Equals(BookingStatus.EXPIRED.ToString()) || booking.StartTime.AddDays(2) >= DateTime.Now)
			{
				throw new BadRequestException(ErrorCode.BOOKING_EXPIRED);
			}
			if (booking.Status.Equals(BookingStatus.ASSIGNED.ToString()))
			{
				throw new BadRequestException(ErrorCode.BOOKING_ALREADY_ASSIGNED);
			}
			//Check doctor available in this time of this booking
			if (!_scheduleSvc.CheckAvailableAssign(doctorId, booking.StartTime, booking.EndTime))
			{
				throw new BadRequestException(ErrorCode.DOCTOR_BUSY);
			}

			//Create schedule for this doctor
			var schedule = new Schedule();
			schedule.BookingId = bookingId;
			schedule.StartTime = booking.StartTime;
			schedule.EndTime = booking.EndTime;
			schedule.UserId = doctorId;
			_scheduleSvc.Create(schedule);
			booking.Status = BookingStatus.ASSIGNED.ToString();
			_bookingRep.Update(booking);
		}

		public async Task ScanAndMakeExpired()
		{
			var bookings = _bookingRep.All;
			if(bookings == null || !bookings.Any())
			{
				return;
			}
			foreach (var booking in bookings)
			{
				if(booking.StartTime.AddDays(2) >= DateTime.Now)
				{
					booking.Status = BookingStatus.EXPIRED.ToString();
					_bookingRep.Update(booking);
				}
			}
		}

		public void AssignBookingDetail(Guid bookingId, Guid bookingDetailId, Guid doctorId)
		{
			//Booking must be have status is waiting_for_assigned for this function.
			var booking = GetById(bookingId);
			if(!booking.Status.Equals(BookingStatus.WAITING_FOR_ASSIGN.ToString()))
			{
				throw new BadRequestException(ErrorCode.STATUS_NOT_ACCEPT);
			}
			//Check boongking details with this booking
			var bookingDetails = _bookingDetailSvc.GetByBookingId(bookingId);
			var bookingDetail = bookingDetails.FirstOrDefault(item => item.Id == bookingDetailId);
			if(bookingDetail == null)
			{
				throw new BadRequestException(ErrorCode.BOOKING_DETAIL_NOT_FOUND);
			}
			//Check is assign before 
			if (!bookingDetail.DoctorId.ToString().Equals("00000000-0000-0000-0000-000000000000"))
			{
				throw new BadRequestException(ErrorCode.BOOKING_ALREADY_ASSIGNED);
			}
			//assgin doctor
			bookingDetail.DoctorId = doctorId;
			//If this is the last booking details, change booking status to Assigned
			var assignedList = bookingDetails
				.Where(item => !item.DoctorId.ToString().Equals("00000000-0000-0000-0000-000000000000"))
				.Count();
			if(assignedList == bookingDetails.Count())
			{
				booking.Status = BookingStatus.ASSIGNED.ToString();
				_bookingRep.Update(booking);
			}
			_bookingDetailSvc.Update(bookingDetail);
		}

		public List<BookingDetail> GetBookingDetail(Guid bookingId)
		{
			return _bookingDetailSvc.GetByBookingId(bookingId);
		}
	}
}
