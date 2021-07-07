using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MinMQ.BenchmarkConsole
{
	public class BenchmarkHostedService : IHostedService
	{
		private readonly IHttpClientFactory httpClientFactory;
		private readonly IHostApplicationLifetime hostApplicationLifetime;
		private readonly Benchmarker benchmarker;

		public BenchmarkHostedService
		(
			IHttpClientFactory httpClientFactory,
			IHostApplicationLifetime hostApplicationLifetime,
			Benchmarker benchmarker
		)
		{
			this.httpClientFactory = httpClientFactory;
			this.hostApplicationLifetime = hostApplicationLifetime;
			this.benchmarker = benchmarker;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
			hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
			// hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

			benchmarker.OnComplete += Program.OnCompletedEvent;
			await benchmarker.Start(cancellationToken);
			await StopAsync(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await Task.CompletedTask;
		}

		// private void OnStarted()
		// {
		// }

		private void OnStopping()
		{
		}

		// private void OnStopped()
		// {
		// }
	}
}
