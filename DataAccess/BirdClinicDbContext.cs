using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
	public partial class BirdClinicDbContext : IdentityDbContext<User>
	{

		public BirdClinicDbContext()
		{
		}

		public BirdClinicDbContext(DbContextOptions<BirdClinicDbContext> options) : base(options)
		{

		}

		public virtual DbSet<Service> Services { get; set; } = null!;
		public virtual DbSet<Transaction> Transactions { get; set; } = null!;
		public virtual DbSet<Booking> Bookings { get; set; } = null!;
		public virtual DbSet<BookingDetail> BookingDetails { get; set; } = null!;
		public virtual DbSet<DoctorService> DoctorServices { get; set; } = null!;


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(GetConnectionString());
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		private string GetConnectionString()
		{
			IConfiguration config = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", true, true)
			.Build();
			var strConn = config["ConnectionStrings:ConnectionString"];
			return strConn;
		}
	}
}
