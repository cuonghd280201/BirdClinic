using Bussiness.Dtos.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
	public class Booking
	{
		public Booking() { 
			BookingDetails = new HashSet<BookingDetail>();
		}
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		public Guid UserId { get; set; }

		[Required]
		public double TotalMoney { get; set; }

		[Required]
		public String Status { get; set; }

		public DateTime CretedAt { get; set; } = DateTime.Now;

		[Required]
		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public virtual User User { get; set; }

		public virtual ICollection<BookingDetail> BookingDetails { get; set; }

		public virtual Transaction Transaction { get; set; }

	}
}
