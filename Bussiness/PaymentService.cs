using Bussiness.Dtos.Enum;
using Common.ExceptionHandler.Exceptions;
using DataAccess.Models;

namespace Bussiness
{
	public class PaymentService
	{
		private readonly BookingSvc _bookingSvc;
		private readonly TransactionSvc _transactionSvc;
		public PaymentService(BookingSvc bookingSvc, TransactionSvc transactionSvc)
		{
			_bookingSvc = bookingSvc;
			_transactionSvc = transactionSvc;
		}

		public void bookingPayment(Guid bookingId)
		{
			var booking = _bookingSvc.GetById(bookingId);
			//Check booking status is waiting for confirm 
			if (!booking.Status.Equals(BookingStatus.WAITING_FOR_CONFIRM.ToString()))
			{
				throw new BadRequestException("This booking is not payment");
			}
			//Create transaction
			var transaction = new Transaction();
			transaction.TotalMoney = booking.TotalMoney;
			transaction.UserId = booking.UserId;
			transaction.BookingId = bookingId;
			transaction.CreatedAt = DateTime.Now;
			_transactionSvc.Create(transaction);
			//Update booking status -> waiting for assign
			booking.Status = BookingStatus.WAITING_FOR_ASSIGN.ToString();
			_bookingSvc.Update(booking);
		}

		public void requestPayment(Guid bookingId)
		{
			var booking = _bookingSvc.GetById(bookingId);
			//Check booking status is waiting for payment 
			if (!booking.Status.Equals(BookingStatus.WAITING_FOR_PAYMENT.ToString()))
			{
				throw new BadRequestException("This booking is waiting for confirm");
			}
			booking.Status = BookingStatus.WAITING_FOR_CONFIRM.ToString();
			_bookingSvc.Update(booking);
		}
	}
}
