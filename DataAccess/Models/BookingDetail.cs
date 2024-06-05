using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
	public class BookingDetail
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		public Guid BookingId { get; set; }

		[Required]
		public Guid ServiceId { get; set; }

		public Guid DoctorId { get; set; }

		[Required]
		public double ServicePrice { get; set; }
	
		public virtual Booking Booking { get; set; }
		public virtual Service Service { get; set; }



	}
}
