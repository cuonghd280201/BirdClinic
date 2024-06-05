using AutoMapper;
using DataAccess;
using DataAccess.Models;

namespace Bussiness
{
	public class TransactionSvc
	{
		private readonly IGenericRep<Transaction> _transaction;
		private readonly IMapper _mapper;

		public TransactionSvc(IGenericRep<Transaction> transaction, IMapper mapper)
		{
			_transaction = transaction;
			_mapper = mapper;
		}

		public List<Transaction> GetTransactionsByBooking(Guid bookingId)
		{
			var allTransaction = _transaction.All;
			if (!allTransaction.Any())
			{
				return new List<Transaction>();
			}
			var transactions = allTransaction.Where(transaction => transaction.BookingId == bookingId);
			if (!transactions.Any())
			{
				return new List<Transaction>();
			}
			return transactions.ToList();
		}

		public void Create(Transaction transaction)
		{
			_transaction.Create(transaction);
		}
	}
}
