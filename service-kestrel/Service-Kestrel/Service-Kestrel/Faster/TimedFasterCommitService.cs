using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_Kestrel.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service_Kestrel.Faster
{
	public class FasterCommitHostedService : IHostedService, IDisposable
	{
		private readonly ILogger<FasterCommitHostedService> logger;
		private Timer timer;

		public FasterCommitHostedService(ILogger<FasterCommitHostedService> logger)
		{
			this.logger = logger;
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("Commit Hosted Service running.");

			var state = new FasterCommitState(FasterContext.Instance.Value.Logger, logger);
			var loop = new FasterCommitCallback();
			// var stateTimer = new Timer(loop.Execute, state, 0, 10);
			timer = new Timer(loop.ExecuteAsync, state, TimeSpan.Zero, TimeSpan.FromMilliseconds(5));

			return Task.CompletedTask;
		}

		//private void DoWork(object state)
		//{
		//	executionCount++;

		//	logger.LogInformation("Timed Hosted Service is working. Count: {Count}", executionCount);
		//}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("Timed Hosted' Service is stopping.");
			timer?.Change(Timeout.Infinite, 0);
			// cancellationTokenSourceFactory().Cancel();
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			timer?.Dispose();
		}
	}
}
