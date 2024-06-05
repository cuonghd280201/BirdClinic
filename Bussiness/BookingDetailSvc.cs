using AutoMapper;
using DataAccess;
using DataAccess.Models;

namespace Bussiness
{
	public class BookingDetailSvc
	{

		private readonly IGenericRep<BookingDetail> _bookingDetailRep;
		private readonly ServiceSvc _serviceSvc;
		public BookingDetailSvc(
			IGenericRep<BookingDetail> bookingDetailService,
			ServiceSvc serviceSvc)
		{
			_bookingDetailRep = bookingDetailService;
			_serviceSvc = serviceSvc;
		}

		public void Craete(BookingDetail bookingDetail)
		{
			_bookingDetailRep.Create(bookingDetail);
		}

		public List<BookingDetail> GetByBookingId(Guid bookingId)
		{
			var All = _bookingDetailRep.All;
			if (!All.Any())
			{
				return new List<BookingDetail>();
			}
			var bookingDetails = All.Where(detail => detail.BookingId == bookingId);
			if (!bookingDetails.Any())
			{
				return new List<BookingDetail>();
			}
			return bookingDetails.ToList();
		}

		public List<Service> GetServiceByBooking(Guid bookingId)
		{
			var bookingDetails = GetByBookingId(bookingId);
			if (!bookingDetails.Any())
			{
				return new List<Service>();
			}
			var serviceIds = bookingDetails.Select(item => item.ServiceId).ToList();
			return _serviceSvc.GetByIds(serviceIds);
		}

		internal void Update(BookingDetail bookingDetail)
		{
			_bookingDetailRep.Update(bookingDetail);
		}
	}
}
