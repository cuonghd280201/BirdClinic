using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAccess.Models
{
	public class Service
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public double Price { get; set; }
		[Required]
		public double EstimateTime { get; set; }
		[JsonIgnore]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		[ForeignKey("BookingDetail")]
		public Guid BookingId { get; set; }

		public virtual BookingDetail BookingDetail { get; set; }
		public virtual ICollection<DoctorService> DoctorServices { get; set; }

	}
}
