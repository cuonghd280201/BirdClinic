using Bussiness;
using Bussiness.Error;
using Common.ExceptionHandler.Exceptions;
using DataAccess.IdentityConfig;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Route("api/services")]
	[ApiController]
	public class ServiceController : ControllerBase
	{
		private readonly ServiceSvc _serviceSvc;

		public ServiceController(ServiceSvc serviceSvc) {
			_serviceSvc = serviceSvc;
		}

		[HttpGet]
		public List<Service> GetAll() {
			return _serviceSvc.Get();
		}

		[Authorize(Roles = UserRoles.Doctor)]
		[HttpGet("doctors")]
		public List<DoctorService> GetByDoctor()
		{
			var userId = HttpContext.Session.GetString("UserId");
			if (userId == null)
			{
				throw new BadRequestException(ErrorCode.TOKEN_IS_MISSING);
			}
			return _serviceSvc.GetServiceByDoctorId(Guid.Parse(userId));
		}

		[HttpGet("{serviceId}")]
		public Service GetService([FromRoute] Guid serviceId)
		{
			return _serviceSvc.GetById(serviceId);
		}

		[HttpPost]
		[Authorize(Roles = UserRoles.Admin)]
		public void CreateService(Service service)
		{
			_serviceSvc.CreateService(service);
		}

		[HttpPost("{serviceId}/doctors/{doctorId}/assign")]
		[Authorize(Roles = UserRoles.Admin)]
		public void AssignDoctor([FromRoute] Guid serviceId, [FromRoute] Guid doctorId)
		{
			_serviceSvc.AssignDoctor(doctorId, serviceId);
		}

		[HttpDelete]
		[Authorize(Roles = UserRoles.Admin)]
		public void DeleteService(Guid serviceId) {
			_serviceSvc.Remove(serviceId);
		}

	}
}
