using DataAccess;
using DataAccess.Models;

namespace Bussiness
{
	public class ScheduleSvc
	{
		private readonly IGenericRep<Schedule> _scheduleRep;
		public ScheduleSvc(IGenericRep<Schedule> ScheduleRep)
		{
			_scheduleRep = ScheduleRep;
		}

		public bool CheckAvailableAssign(Guid UserId, DateTime StartTime, DateTime EndTime)
		{
			var all = _scheduleRep.All;
			if (all == null)
			{
				return true;
			}
			var schedules = all.FirstOrDefault(schedule =>
			StartTime < schedule.StartTime && schedule.StartTime < EndTime && schedule.UserId == UserId
			|| StartTime < schedule.EndTime && schedule.EndTime < EndTime && schedule.UserId == UserId);
			if (schedules == null)
			{
				return true;
			}
			return false;
		}

		public void Create(Schedule schedule)
		{
			_scheduleRep.Create(schedule);
		}
	}
}
