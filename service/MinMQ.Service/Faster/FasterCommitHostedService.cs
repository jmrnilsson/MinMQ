using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MinMQ.Service.Faster
{

	internal delegate ValueTask CommitAsyncDelegate(CancellationToken token = default);

	/// <summary>
	/// A hosted service that runs in the background. It's responsible for flushing commits every 5ms. It's required
	/// in high-contention scenarios because the eventloop gets completely bogged down with requests. In such scenarios
	/// all threads get stuck at some other step than FasterLog.CommitAsync(). However, this service's Timer is always
	/// granted some CPU-time.
	/// </summary>
	public class FasterCommitHostedService : IHostedService, IDisposable
	{
		private const int PeriodMs = 5;
		private const int LogCommitTimerEverySeconds = 3000;
		private readonly ILogger<FasterCommitHostedService> logger;
		private Timer timer;

		public FasterCommitHostedService(ILogger<FasterCommitHostedService> logger)
		{
			this.logger = logger;
		}

		private static async void ExecuteAsync(object stateInfo)
		{
			FasterCommitState state = stateInfo as FasterCommitState;

			await state.CommitAsync();

			if (state.InvokationCount > 0 && state.InvokationCount % state.LoggingInterval == 0)
			{
				state.InvokationCount = 0;
				state.Logger.LogInformation("Polling commits. state.InvokationCount % {0} == 0", state.LoggingInterval);
			}
			else
			{
				state.InvokationCount++;
			}
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("Commit Hosted Service running.");

			var state = new FasterCommitState(FasterOps.Instance.Value.CommitAsync, logger, GetLoggingInterval());
			timer = new Timer(ExecuteAsync, state, TimeSpan.Zero, TimeSpan.FromMilliseconds(PeriodMs));

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("Timed Hosted' Service is stopping.");
			timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			timer?.Dispose();
		}

		private int GetLoggingInterval()
		{
			int interval = LogCommitTimerEverySeconds / PeriodMs;
			interval = Math.Max(1, interval);
			return Math.Min(2000, interval);
		}

		private class FasterCommitState
		{
			public FasterCommitState(CommitAsyncDelegate commitAsync, ILogger logger, int loggingInterval)
			{
				CommitAsync = commitAsync;
				Logger = logger;
				LoggingInterval = loggingInterval;
				InvokationCount = 0;
			}

			public CommitAsyncDelegate CommitAsync { get; }
			public ILogger Logger { get; }
			public int LoggingInterval { get; }
			public int InvokationCount { get; set; }
		}
	}
}
