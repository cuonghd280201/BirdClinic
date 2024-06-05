using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
	public class Transaction
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		public Guid UserId { get; set; }

		[ForeignKey("Booking")]
		[Required]
		public Guid BookingId { get; set; }

		[Required]
		public double TotalMoney { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; }

		
		public virtual Booking Booking { get; set; }
	}
}
