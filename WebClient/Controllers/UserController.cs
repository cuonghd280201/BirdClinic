using Common.ExceptionHandler;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WebClient.Models;
using WebClient.Models.Requests;
using WebClient.Models.Token;

namespace WebClient.Controllers
{
	public class UserController : Controller
	{
		private readonly HttpClient _httpClient;
		public UserController(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
		}
	
		public String GetCookie(string cookieName)
		{
			if (Request.Cookies[cookieName] != null)
			{
				return Request.Cookies[cookieName];
			}
			return null;
		}

		public async Task<IActionResult> ViewBooking(int page, int size)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = $"http://localhost:5000/api/bookings?page={page}&size={size}&userId={GetCookie("UserId")}";
			var response = await _httpClient.GetAsync(url);
			var viewModel = new ListBooking();
			string jsonResponse = await response.Content.ReadAsStringAsync();
			List<Booking> books = JsonConvert.DeserializeObject<List<Booking>>(jsonResponse);
			viewModel.Bookings = books;
			return View(viewModel);

		}
		public async Task<IActionResult> ViewBookingDetail(Guid bookingId)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = $"http://localhost:5000/api/bookings/{bookingId}/details";
            var response = await _httpClient.GetAsync(url);
			var viewModel = new ListBookingDetail();
			string jsonResponse = await response.Content.ReadAsStringAsync();
			List<BookingDetail> bookDetails = JsonConvert.DeserializeObject<List<BookingDetail>>(jsonResponse);
			viewModel.BookingDetails = bookDetails;
			return View(viewModel);

		}
		public async Task<IActionResult> Service()
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = $"http://localhost:5000/api/services";
			var reponse = await _httpClient.GetAsync(url);
			var viewModel = new ListService();
			string jsonService = await reponse.Content.ReadAsStringAsync();
            List<Service> services = JsonConvert.DeserializeObject<List<Service>>(jsonService);
			ViewBag.Services = services!.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name,
			}).ToList();
            return View(viewModel);
		}
		[HttpGet]
		public async Task<IActionResult> CreateDetail()
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = "http://localhost:5000/api/services";
			var response = await _httpClient.GetAsync(url);
			string jsonService = await response.Content.ReadAsStringAsync();
			List<Service> services = JsonConvert.DeserializeObject<List<Service>>(jsonService);
			ViewBag.Services = services.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name,
			}).ToList();
		
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateDetail(Guid bookingId, Guid serviceId, AddServiceRequest request)
		{
			if (!ModelState.IsValid)
			{
				var serviceResponse = await _httpClient.GetAsync("http://localhost:5000/api/services");
				string jsonService = await serviceResponse.Content.ReadAsStringAsync();
				List<Service> services = JsonConvert.DeserializeObject<List<Service>>(jsonService);
				ViewBag.Services = services.Select(s => new SelectListItem
				{
					Value = s.Id.ToString(),
					Text = s.Name,
				}).ToList();
				return View(request);
				ViewBag.BookingId = bookingId;

			}
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = $"http://localhost:5000/api/bookings/{bookingId}/services/{serviceId}";
			var jsonRequest = JsonConvert.SerializeObject(request);
			var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(url, content);
			return RedirectToAction("ViewBookingDetail", "User");
			

		}

		[HttpGet]
        public async Task<IActionResult> Create()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
            var url = "http://localhost:5000/api/services";
            var response = await _httpClient.GetAsync(url);
            string jsonService = await response.Content.ReadAsStringAsync();
            List<Service> services = JsonConvert.DeserializeObject<List<Service>>(jsonService);
            ViewBag.Services = services.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
            }).ToList();
            return View();
        }
        [HttpPost]
		public async Task<IActionResult> Create(AddServiceRequest request)
		{
			if (!ModelState.IsValid)
			{
				var serviceResponse = await _httpClient.GetAsync("http://localhost:5000/api/services");
				string jsonService = await serviceResponse.Content.ReadAsStringAsync();
				List<Service> services = JsonConvert.DeserializeObject<List<Service>>(jsonService);
				ViewBag.Services = services.Select(s => new SelectListItem
				{
					Value = s.Id.ToString(),
					Text = s.Name,
				}).ToList();
				return View(request);

			}
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = $"http://localhost:5000/api/bookings/services";
			var jsonRequest = JsonConvert.SerializeObject(request);
			var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(url, content);
			return RedirectToAction("ViewBooking", "User");
			
			
		}

		public async Task<IActionResult> Payment(Guid bookingId)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = $"http://localhost:5000/api/payments/booking/{bookingId}/request";
			var response = await _httpClient.PutAsync(url, null);
			if (response.IsSuccessStatusCode)
			{
				return Ok("Payment request sent successfully.");
			}
			else
			{
				return RedirectToAction("ViewBookingDetail", "User");
			}

		}

		public async Task<IActionResult> AddService(Guid bookingId, Guid serviceId)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetCookie("Token"));
			var url = $"http://localhost:5000/api/bookings/{bookingId}/services/{serviceId}";
			var response = await _httpClient.PostAsync(url, null);
			if (response.IsSuccessStatusCode)
			{
				return RedirectToAction("BookingDetails", new { bookingId });

			}
			else
			{
				return RedirectToAction("Error");
			}

		}

	}
}
