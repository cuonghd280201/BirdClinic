using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Bussiness;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using Bussiness.Config;
using Bussiness.Middleware;
using Bussiness.Utils;
using Microsoft.OpenApi.Models;
using Common.ExceptionHandler;
using Bussiness.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDistributedMemoryCache(); // Ví dụ: Triển khai bộ nhớ trong cache
builder.Services.AddDistributedMemoryCache();

// Cấu hình lưu trữ phiên
builder.Services.AddSession(options =>
{
	options.Cookie.Name = "YourSessionCookieName";
	options.IdleTimeout = TimeSpan.FromMinutes(20);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<BirdClinicDbContext>(
	options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<BirdClinicDbContext>()
	   .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
			.AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = "user",
					ValidIssuer = "nam@gmail.com",
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"))
				};
			});

IConfiguration configuration = new ConfigurationBuilder()
		.SetBasePath(AppContext.BaseDirectory)
		.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		.Build();

builder.Services.AddSingleton(configuration);

builder.Services.AddSingleton<TokenUtils>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<TokenMiddleware>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<PaymentService>();

builder.Services.AddTransient<BookingSvc>();
builder.Services.AddTransient<IGenericRep<Booking>, GenericRep<BirdClinicDbContext, Booking>>();

builder.Services.AddHostedService<ScanExpiredBookingJob>();

builder.Services.AddTransient<BookingDetailSvc>();
builder.Services.AddTransient<IGenericRep<BookingDetail>, GenericRep<BirdClinicDbContext, BookingDetail>>();

builder.Services.AddTransient<TransactionSvc>();
builder.Services.AddTransient<IGenericRep<Transaction>, GenericRep<BirdClinicDbContext, Transaction>>();

builder.Services.AddTransient<IGenericRep<DoctorService>, GenericRep<BirdClinicDbContext, DoctorService>>();

builder.Services.AddTransient<ServiceSvc>();
builder.Services.AddTransient<IGenericRep<Service>, GenericRep<BirdClinicDbContext, Service>>();

builder.Services.AddTransient<ScheduleSvc>();
builder.Services.AddTransient<IGenericRep<Schedule>, GenericRep<BirdClinicDbContext, Schedule>>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseMiddleware<TokenMiddleware>();

app.MapControllers();

app.Run();
