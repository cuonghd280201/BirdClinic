using AutoMapper;

namespace Bussiness.Config
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			//CreateMap<FlowerBouquet, FlowerBouquet>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		}
	}
}
