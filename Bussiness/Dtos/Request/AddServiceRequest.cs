using System.Text.Json.Serialization;

namespace Bussiness.Dtos.Request
{
	public class AddServiceRequest
	{
		public Guid ServiceId { get; set; }
		[JsonIgnore]
		public Guid UserId { get; set; }
		public DateTime StartAt { get; set; }
	}
}
