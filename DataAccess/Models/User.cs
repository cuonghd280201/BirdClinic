using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public class User : IdentityUser
	{
		public User() { 
			Bookings = new HashSet<Booking>();
			DoctorServices = new HashSet<DoctorService>();
		}
		public virtual ICollection<Booking> Bookings { get; set;} 
		public virtual ICollection<DoctorService> DoctorServices { get; set;} 
	}

}
