using Microsoft.Extensions.Hosting;

namespace Bussiness.Jobs
{
	public class ScanExpiredBookingJob : IHostedService, IDisposable
	{
		private Timer _timer;

		private readonly BookingSvc _bookingSvc;
		public ScanExpiredBookingJob(BookingSvc bookingSvc)
		{
			_bookingSvc = bookingSvc;
		}
		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
			return Task.CompletedTask;
		}

		private void DoWork(object state)
		{
			_bookingSvc.ScanAndMakeExpired();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
