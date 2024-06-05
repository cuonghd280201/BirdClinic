using Common.ExceptionHandler.Exceptions;
using DataAccess;
using DataAccess.Models;

namespace Bussiness
{
	public class ServiceSvc
	{
		private readonly IGenericRep<Service> _serviceRep;
		private readonly IGenericRep<DoctorService> _doctorServiceRep;

		public ServiceSvc(IGenericRep<Service> serviceRep, IGenericRep<DoctorService> doctorServiceRep)
		{
			_serviceRep = serviceRep;
			_doctorServiceRep = doctorServiceRep;
		}

		public List<DoctorService> GetServiceByDoctorId(Guid doctorId)
		{
			var all = _doctorServiceRep.All;
			if (all == null || !all.Any())
			{
				return new List<DoctorService>();
			}

			var result = all.Where(service => service.UserId == doctorId);
			if (!result.Any())
			{
				return new List<DoctorService>();
			}
			return result.ToList();
		}

		public void AssignDoctor(Guid doctorId, Guid serviceId)
		{
			var doctorService = new DoctorService();
			doctorService.UserId = doctorId;
			doctorService.ServiceId = serviceId;
			_doctorServiceRep.Create(doctorService);
		}

		public void CreateService(Service service)
		{
			_serviceRep.Create(service);
		}

		public List<Service> Get()
		{
			var services = _serviceRep.All;
			if (services == null)
			{
				return new List<Service>();
			}
			return services.ToList();
		}

		public Service GetById(Guid serviceId)
		{
			var services = _serviceRep.All;
			if (services == null)
			{
				throw new BadRequestException("Service not found");
			}

			var response = services.FirstOrDefault(service => serviceId == service.Id);
			if (response == null)
			{
				throw new BadRequestException("Service not found");
			}
			return response; 
		}

		public List<Service> GetByIds(List<Guid> serviceIds)
		{
			var services = _serviceRep.All;
			if (services == null)
			{
				return new List<Service>();
			}

			var response = services.Where(service => serviceIds.Contains(service.Id));
			return response.ToList();
		}

		public void Remove(Guid serviceId)
		{
			var service = GetById(serviceId);
			if (service == null)
			{
				throw new BadRequestException("Service not found");
			}
			_serviceRep.Delete(service);
		}
	}
}
